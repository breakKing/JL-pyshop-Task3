using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Configurations;

public class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        // Составной первичный ключ
        builder.HasKey(m => new { m.UnixTimestamp, m.CoinId });

        // Индекс по CoinId
        builder.HasIndex(m => m.CoinId);

        // Один ко многим к User (SrcUserId)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.SrcUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Один ко многим к User (DstUserId)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.DstUserId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        // Один ко многим к Coin
        builder.HasOne<Coin>()
            .WithMany(c => c.Moves)
            .HasForeignKey(m => m.CoinId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
