using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeMart.Api.Data.Configurations;

public class InvoiceLockConfiguration : IEntityTypeConfiguration<InvoiceLock>
{
    public void Configure(EntityTypeBuilder<InvoiceLock> builder)
    {
        builder.ToTable("TrInvoiceLock");
        builder.HasKey(x => x.IdInvoiceLock);
        builder.Property(x => x.InvoiceNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.IdUser).HasMaxLength(5).IsRequired();
        builder.Property(x => x.LockDateOnly).HasColumnType("date");
        builder.Property(x => x.Userin).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Userup).HasMaxLength(36);
        builder.HasIndex(x => x.InvoiceNo).IsUnique();
        builder.HasIndex(x => new { x.IdUser, x.LockDateOnly }).IsUnique();
        builder.HasIndex(x => x.LockDateOnly);
        builder.HasOne(x => x.User)
            .WithMany(x => x.InvoiceLocks)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
