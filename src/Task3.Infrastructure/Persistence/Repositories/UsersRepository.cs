using Microsoft.EntityFrameworkCore;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Repositories;

public class UsersRepository : GenericRepository<User, long>, IUsersRepository
{
    protected override Func<User, long> GetEntityId => (u => u.Id);

    public UsersRepository(BillingDbContext context) : base(context)
    {
        
    }

    public async Task<List<TProjection>> GetAllWithSortedRatingAsync<TProjection>(
        Func<User, TProjection> projection,
        CancellationToken ct = default)
    {
        return await Context.Users
            .OrderByDescending(u => u.Rating)
            .Select(u => projection(u))
            .ToListAsync(ct);
    }

    public async Task<List<User>> GetAllWithCoinsAsync(CancellationToken ct = default)
    {
        return await Context.Users
            .Include(u => u.Coins)
            .AsSplitQuery()
            .ToListAsync(ct);
    }

    public async Task<User?> GetOneWithCoinsAsync(string userName,
        CancellationToken ct = default)
    {
        return await Context.Users
            .Include(u => u.Coins)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Name == userName, ct);
    }
}
