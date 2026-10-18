using BeeMart.Api.Data;
using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Repositories;

public class ProductRepository
{
    private readonly BeeMartDbContext _db;

    public ProductRepository(BeeMartDbContext db)
    {
        _db = db;
    }

    public Task<List<Product>> SearchAsync(string keyword, int limit, CancellationToken cancellationToken)
    {
        return _db.Products
            .Where(x => x.Stsrc && x.Name.Contains(keyword))
            .OrderBy(x => x.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Product>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        var list = ids.Distinct().ToList();
        return _db.Products.Where(x => list.Contains(x.IdProduct)).ToListAsync(cancellationToken);
    }

    public Task<List<PaymentMethod>> GetPaymentMethodsAsync(CancellationToken cancellationToken)
    {
        return _db.PaymentMethods.Where(x => x.Stsrc).OrderBy(x => x.IdPaymentMethod).ToListAsync(cancellationToken);
    }
}
