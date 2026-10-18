using BeeMart.Api.Data;
using BeeMart.Api.Data.Entities;
using BeeMart.Api.Dtos;
using BeeMart.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Services;

public class InvoiceService
{
    private readonly InvoiceLockRepository _locks;
    private readonly BeeMartDbContext _db;

    public InvoiceService(InvoiceLockRepository locks, BeeMartDbContext db)
    {
        _locks = locks;
        _db = db;
    }

    public async Task<InvoiceLockResponse> EnsureLockAsync(string idUser, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var today = now.Date;

        var expired = await _locks.GetExpiredAsync(idUser, today, cancellationToken);
        if (expired.Count > 0)
        {
            await _locks.RemoveRangeAsync(expired, cancellationToken);
        }

        var existing = await _locks.FindAsync(idUser, today, cancellationToken);
        if (existing is not null)
        {
            return new InvoiceLockResponse(existing.InvoiceNo, existing.LockDate, today.AddDays(1));
        }

        var todayInvoices = await _db.Transactions
            .Where(x => x.IdUser == idUser && x.TransactionDate.Date == today)
            .Select(x => x.InvoiceNo)
            .ToListAsync(cancellationToken);

        var maxTransactionRunning = todayInvoices
            .Select(x => int.TryParse(x.Substring(Math.Max(0, x.Length - 3)), out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        var maxLockRunning = await _locks.MaxRunningNumberAsync(idUser, today, cancellationToken);
        var nextRunning = Math.Max(maxTransactionRunning, maxLockRunning) + 1;

        if (nextRunning > 999)
        {
            throw new ConflictException("INVOICE_RUNNING_EXCEEDED", "Nomor invoice untuk hari ini sudah mencapai batas.");
        }

        var invoiceNo = idUser + now.ToString("ddMMyy") + nextRunning.ToString("D3");
        var entity = new InvoiceLock
        {
            IdInvoiceLock = Guid.NewGuid(),
            InvoiceNo = invoiceNo,
            IdUser = idUser,
            LockDate = now,
            LockDateOnly = today,
            RunningNumber = nextRunning,
            Stsrc = true,
            Datein = now,
            Userin = idUser
        };

        await _locks.AddAsync(entity, cancellationToken);

        return new InvoiceLockResponse(entity.InvoiceNo, entity.LockDate, today.AddDays(1));
    }
}
