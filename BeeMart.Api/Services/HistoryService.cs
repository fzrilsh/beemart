using BeeMart.Api.Data;
using BeeMart.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Services;

public class HistoryService
{
    private const int PageSize = 20;
    private readonly BeeMartDbContext _db;

    public HistoryService(BeeMartDbContext db)
    {
        _db = db;
    }

    public async Task<HistoryResponse> GetAsync(string idUser, string role, DateTime date, int page, CancellationToken cancellationToken)
    {
        if (page < 1)
        {
            page = 1;
        }

        var query = _db.Transactions
            .Include(x => x.User)
            .Include(x => x.PaymentMethod)
            .Include(x => x.Details)
            .Where(x => x.TransactionDate.Date == date.Date);

        if (role != "Admin")
        {
            query = query.Where(x => x.IdUser == idUser);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.TransactionDate)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var rows = items
            .Select(x => new HistoryItemDto(
                x.InvoiceNo,
                x.TransactionDate,
                x.User.FullName,
                x.Details.Sum(d => d.Qty),
                x.PaymentMethod.MethodName,
                x.TotalPrice))
            .ToList();

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)PageSize);
        return new HistoryResponse(rows, page, PageSize, totalItems, totalPages);
    }
}
