using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeMart.Api.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("MsProduct");
        builder.HasKey(x => x.IdProduct);
        builder.Property(x => x.IdProduct).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Weight).HasPrecision(10, 2);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.Discount).HasPrecision(5, 2);
        builder.Property(x => x.Userin).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Userup).HasMaxLength(36);
        builder.HasIndex(x => x.Name);
    }
}
