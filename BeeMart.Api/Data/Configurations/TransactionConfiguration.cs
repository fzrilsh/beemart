using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeMart.Api.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("TrTransaction");
        builder.HasKey(x => x.IdTransaction);
        builder.Property(x => x.IdTransaction).HasMaxLength(36).IsRequired();
        builder.Property(x => x.InvoiceNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.IdUser).HasMaxLength(5).IsRequired();
        builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
        builder.Property(x => x.Userin).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Userup).HasMaxLength(36);
        builder.HasIndex(x => x.InvoiceNo).IsUnique();
        builder.HasIndex(x => x.TransactionDate);
        builder.HasIndex(x => x.IdUser);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.PaymentMethod)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.IdPaymentMethod)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
