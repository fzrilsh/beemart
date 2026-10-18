using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeMart.Api.Data.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("LtPaymentMethod");
        builder.HasKey(x => x.IdPaymentMethod);
        builder.Property(x => x.MethodName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Userin).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Userup).HasMaxLength(36);
    }
}
