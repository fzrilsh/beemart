using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeMart.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("MsUser");
        builder.HasKey(x => x.IdUser);
        builder.Property(x => x.IdUser).HasMaxLength(5).IsRequired();
        builder.Property(x => x.Username).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(300).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Userin).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Userup).HasMaxLength(36);
        builder.HasIndex(x => x.Username).IsUnique();
    }
}
