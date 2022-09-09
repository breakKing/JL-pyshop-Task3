using Task3.Domain.Entities;

namespace Task3.Application.Common.Interfaces.Repositories;

public interface ICoinsRepository : IGenericRepository<Coin, long>
{
    Task<bool> AddMovesAsync(
        long srcUserId,
        long dstUserId,
        long amount = 1,
        CancellationToken ct = default);
}
