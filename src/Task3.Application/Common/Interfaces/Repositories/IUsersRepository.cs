using Task3.Domain.Entities;

namespace Task3.Application.Common.Interfaces.Repositories;

public interface IUsersRepository : IGenericRepository<User, long>
{
    Task<User?> GetOneWithCoinsAsync(Func<User, bool> filter,
        CancellationToken ct = default);

    Task<List<User>> GetAllWithCoinsAsync(CancellationToken ct = default);

    Task<List<TProjection>> GetAllWithSortedRatingAsync<TProjection>(
        Func<User, TProjection> projection,
        CancellationToken ct = default);
}
