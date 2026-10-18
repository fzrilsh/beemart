using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Data;

public class BeeMartDbContext : DbContext
{
    public BeeMartDbContext(DbContextOptions<BeeMartDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionDetail> TransactionDetails => Set<TransactionDetail>();
    public DbSet<InvoiceLock> InvoiceLocks => Set<InvoiceLock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeeMartDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
