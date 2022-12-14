using Task3.Domain.Entities;

namespace Task3.Application.Common.Interfaces.Repositories;

public interface ICoinsRepository : IGenericRepository<Coin, long>
{
    Task<List<TProjection>> GetUserCoinsAsync<TProjection>(
        Func<Coin, TProjection> projection,
        long userId,
        CancellationToken ct = default);

    Task<bool> AddCoinsToUserAsync(
        long userId,
        long amount,
        CancellationToken ct = default);

    Task<bool> AddMovesAsync(
        long srcUserId,
        long dstUserId,
        long amount = 1,
        CancellationToken ct = default);

    Task<TProjection?> GetCoinWithLongestHistoryOrDefaultAsync<TProjection>(
        Func<Coin, TProjection> projection,
        CancellationToken ct = default
    );
}
