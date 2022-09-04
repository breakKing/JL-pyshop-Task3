using Microsoft.EntityFrameworkCore;
using Task3.Application.Common.Interfaces;

namespace Task3.Infrastructure.Persistence.Repositories;

public abstract class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : IEquatable<TKey>
{
    protected BillingDbContext Context { get; }

    protected abstract Func<TEntity, TKey> GetEntityId { get; }

    protected GenericRepository(BillingDbContext context)
    {
        Context = context;
    }

    public virtual async Task<TKey?> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Context.Set<TEntity>()
            .AddAsync(entity, ct);

        var saveResult = await Context.SaveChangesAsync(ct);

        if (saveResult > 0)
        {
            return GetEntityId(entity);
        }

        return default(TKey?);
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await Context.Set<TEntity>()
            .ToListAsync(ct);
    }
}
