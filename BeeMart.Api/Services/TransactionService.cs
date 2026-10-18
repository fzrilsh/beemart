using BeeMart.Api.Data;
using BeeMart.Api.Data.Entities;
using BeeMart.Api.Dtos;
using BeeMart.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Services;

public class TransactionService
{
    private readonly BeeMartDbContext _db;
    private readonly ProductRepository _products;
    private readonly InvoiceService _invoices;

    public TransactionService(BeeMartDbContext db, ProductRepository products, InvoiceService invoices)
    {
        _db = db;
        _products = products;
        _invoices = invoices;
    }

    public async Task<CalculateResponse> CalculateAsync(List<CartItemRequest>? items, CancellationToken cancellationToken)
    {
        var list = items ?? new List<CartItemRequest>();
        if (list.Count == 0)
        {
            return new CalculateResponse(new List<CartLineDto>(), 0);
        }

        var ids = list.Select(x => x.IdProduct).Distinct().ToList();
        var products = await _products.GetByIdsAsync(ids, cancellationToken);
        var lines = new List<CartLineDto>();

        foreach (var item in list)
        {
            var product = products.FirstOrDefault(x => x.IdProduct == item.IdProduct);
            if (product is null)
            {
                throw new ValidationException("Produk tidak ditemukan.");
            }
            if (item.Qty < 1)
            {
                throw new ValidationException("Qty minimal 1.");
            }
            if (item.Qty > product.Stock)
            {
                throw new ConflictException("STOCK_NOT_ENOUGH", "Jumlah barang yang dimasukkan melebihi sisa stok.");
            }

            var gross = product.Price * item.Qty;
            var subTotal = gross - gross * (product.Discount / 100m);
            lines.Add(new CartLineDto(product.IdProduct, product.Name, product.Price, product.Discount, item.Qty, Math.Round(subTotal, 2)));
        }

        return new CalculateResponse(lines, lines.Sum(x => x.SubTotal));
    }

    public async Task<SubmitTransactionResponse> SubmitAsync(string idUser, SubmitTransactionRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items ?? new List<CartItemRequest>();
        if (items.Count == 0)
        {
            throw new ValidationException("Keranjang masih kosong.");
        }

        var today = DateTime.Now.Date;
        var lockEntity = await _db.InvoiceLocks
            .FirstOrDefaultAsync(x => x.IdUser == idUser && x.InvoiceNo == request.InvoiceNo, cancellationToken);

        string invoiceNo;
        if (lockEntity is null || lockEntity.LockDateOnly < today)
        {
            if (lockEntity is not null)
            {
                _db.InvoiceLocks.Remove(lockEntity);
                await _db.SaveChangesAsync(cancellationToken);
            }

            var fresh = await _invoices.EnsureLockAsync(idUser, cancellationToken);
            invoiceNo = fresh.InvoiceNo;
            lockEntity = await _db.InvoiceLocks
                .FirstOrDefaultAsync(x => x.IdUser == idUser && x.InvoiceNo == invoiceNo, cancellationToken);
        }
        else
        {
            invoiceNo = lockEntity.InvoiceNo;
        }

        var ids = items.Select(x => x.IdProduct).Distinct().ToList();
        var products = await _products.GetByIdsAsync(ids, cancellationToken);

        foreach (var item in items)
        {
            var product = products.FirstOrDefault(x => x.IdProduct == item.IdProduct);
            if (product is null)
            {
                throw new ValidationException("Produk tidak ditemukan.");
            }
            if (item.Qty < 1)
            {
                throw new ValidationException("Qty minimal 1.");
            }
            if (item.Qty > product.Stock)
            {
                throw new ConflictException("STOCK_NOT_ENOUGH", "Jumlah barang yang dimasukkan melebihi sisa stok.");
            }
        }

        var paymentExists = await _db.PaymentMethods.AnyAsync(x => x.IdPaymentMethod == request.IdPaymentMethod, cancellationToken);
        if (!paymentExists)
        {
            throw new ValidationException("Metode pembayaran tidak valid.");
        }

        var now = DateTime.Now;
        var transaction = new Transaction
        {
            IdTransaction = Guid.NewGuid().ToString(),
            InvoiceNo = invoiceNo,
            TransactionDate = now,
            IdUser = idUser,
            IdPaymentMethod = request.IdPaymentMethod,
            Stsrc = true,
            Datein = now,
            Userin = idUser
        };

        decimal total = 0;
        foreach (var item in items)
        {
            var product = products.First(x => x.IdProduct == item.IdProduct);
            var gross = product.Price * item.Qty;
            var subTotal = Math.Round(gross - gross * (product.Discount / 100m), 2);
            total += subTotal;
            product.Stock -= item.Qty;
            transaction.Details.Add(new TransactionDetail
            {
                IdTransactionDetail = Guid.NewGuid().ToString(),
                IdTransaction = transaction.IdTransaction,
                IdProduct = product.IdProduct,
                Qty = item.Qty,
                Price = product.Price,
                Discount = product.Discount,
                SubTotal = subTotal,
                Stsrc = true,
                Datein = now,
                Userin = idUser
            });
        }

        transaction.TotalPrice = total;
        _db.Transactions.Add(transaction);

        if (lockEntity is not null)
        {
            _db.InvoiceLocks.Remove(lockEntity);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new SubmitTransactionResponse(transaction.InvoiceNo, transaction.TransactionDate, transaction.TotalPrice);
    }
}
