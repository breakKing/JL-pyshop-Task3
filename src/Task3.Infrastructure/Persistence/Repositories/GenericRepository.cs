using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Task3.Application.Common.Interfaces.Repositories;

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

        var saveResult = await SaveDataAsync(ct);

        if (saveResult)
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

    public virtual async Task<List<TProjection>> GetAllAsync<TProjection>(
        Func<TEntity, TProjection> projection,
        CancellationToken ct = default)
    {
        return await Context.Set<TEntity>()
            .Select(e => projection(e))
            .ToListAsync(ct);
    }

    public virtual async Task<bool> RemoveAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Context.Set<TEntity>()
            .FirstOrDefaultAsync(e => GetEntityId(e).Equals(id), ct);

        if (entity is null)
        {
            return true;
        }

        Context.Remove(entity);
        var saveResult = await SaveDataAsync(ct);

        return saveResult;
    }

    protected async Task<bool> SaveDataAsync(CancellationToken ct = default)
    {
        int saveResult;

        try
        {
            saveResult = await Context.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            return false;
        }

        return saveResult > 0;
    }

    public async Task<TProjection?> GetOneAsync<TProjection>(
        Func<TEntity, TProjection> projection,
        TKey id,
        CancellationToken ct = default)
    {
        return await Context.Set<TEntity>()
            .Where(e => GetEntityId(e).Equals(id))
            .Select(e => projection(e))
            .FirstOrDefaultAsync(ct);
    }
}
