using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeMart.Api.Data.Configurations;

public class TransactionDetailConfiguration : IEntityTypeConfiguration<TransactionDetail>
{
    public void Configure(EntityTypeBuilder<TransactionDetail> builder)
    {
        builder.ToTable("TrTransactionDetail");
        builder.HasKey(x => x.IdTransactionDetail);
        builder.Property(x => x.IdTransactionDetail).HasMaxLength(36).IsRequired();
        builder.Property(x => x.IdTransaction).HasMaxLength(36).IsRequired();
        builder.Property(x => x.IdProduct).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.Discount).HasPrecision(5, 2);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.Userin).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Userup).HasMaxLength(36);
        builder.HasIndex(x => x.IdTransaction);
        builder.HasIndex(x => x.IdProduct);
        builder.HasOne(x => x.Transaction)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.IdTransaction)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Product)
            .WithMany(x => x.TransactionDetails)
            .HasForeignKey(x => x.IdProduct)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
