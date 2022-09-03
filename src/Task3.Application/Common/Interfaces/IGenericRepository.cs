namespace Task3.Application.Common.Interfaces;

public interface IGenericRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : IEquatable<TKey>
{
    Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TKey?> AddAsync(TEntity entity, CancellationToken ct = default);
}
