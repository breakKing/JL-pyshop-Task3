using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Ограничение длины имени
        builder.Property(u => u.Name)
            .HasMaxLength(150);
    }
}
