using BeeMart.Api.Data;
using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Repositories;

public class InvoiceLockRepository
{
    private readonly BeeMartDbContext _db;

    public InvoiceLockRepository(BeeMartDbContext db)
    {
        _db = db;
    }

    public Task<InvoiceLock?> FindAsync(string idUser, DateTime dateOnly, CancellationToken cancellationToken)
    {
        return _db.InvoiceLocks.FirstOrDefaultAsync(x => x.IdUser == idUser && x.LockDateOnly == dateOnly, cancellationToken);
    }

    public Task<List<InvoiceLock>> GetExpiredAsync(string idUser, DateTime today, CancellationToken cancellationToken)
    {
        return _db.InvoiceLocks.Where(x => x.IdUser == idUser && x.LockDateOnly < today).ToListAsync(cancellationToken);
    }

    public async Task<int> MaxRunningNumberAsync(string idUser, DateTime today, CancellationToken cancellationToken)
    {
        var value = await _db.InvoiceLocks
            .Where(x => x.IdUser == idUser && x.LockDateOnly == today)
            .Select(x => (int?)x.RunningNumber)
            .MaxAsync(cancellationToken);
        return value ?? 0;
    }

    public async Task AddAsync(InvoiceLock invoiceLock, CancellationToken cancellationToken)
    {
        _db.InvoiceLocks.Add(invoiceLock);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(IEnumerable<InvoiceLock> locks, CancellationToken cancellationToken)
    {
        _db.InvoiceLocks.RemoveRange(locks);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
