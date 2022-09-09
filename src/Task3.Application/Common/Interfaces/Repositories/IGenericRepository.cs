namespace Task3.Application.Common.Interfaces.Repositories;

public interface IGenericRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : IEquatable<TKey>
{
    Task<TProjection?> GetOneAsync<TProjection>(Func<TEntity, TProjection> projection,
        Func<TEntity, bool> filter,
        CancellationToken ct = default);

    Task<List<TProjection>> GetManyAsync<TProjection>(Func<TEntity, TProjection> projection,
        Func<TEntity, bool> filter,
        CancellationToken ct = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);

    Task<List<TProjection>> GetAllAsync<TProjection>(Func<TEntity, TProjection> projection,
        CancellationToken ct = default);

    Task<TKey?> AddAsync(TEntity entity, CancellationToken ct = default);

    Task<bool> RemoveAsync(TKey id, CancellationToken ct = default);
}
