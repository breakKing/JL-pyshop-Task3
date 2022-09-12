using Microsoft.EntityFrameworkCore;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Application.Common.Interfaces.Services;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Repositories;

public class CoinsRepository : GenericRepository<Coin, long>, ICoinsRepository
{
    private readonly IDateTimeService _dateTimeService;

    protected override Func<Coin, long> GetEntityId => (c => c.Id);

    public CoinsRepository(BillingDbContext context, IDateTimeService dateTimeService) :
        base(context)
    {
        _dateTimeService = dateTimeService;
    }

    public async Task<bool> AddCoinsToUserAsync(
        long userId,
        long amount,
        CancellationToken ct = default)
    {
        var coins = new List<Coin>();
        for (var i = 0; i < amount; i++)
        {
            var coin = new Coin
            {
                UserId = userId
            };
            
            coins.Add(coin);
        }

        await Context.Coins.AddRangeAsync(coins, ct);
        bool saveResult = await SaveDataAsync(ct);
        
        if (!saveResult)
        {
            return false;
        }

        var moves = new List<Move>();
        for (var i = 0; i < amount; i++)
        {
            var move = new Move
            {
                CoinId = coins[i].Id,
                UnixTimestamp = _dateTimeService.UtcNowOffset.ToUnixTimeMilliseconds(),
                SrcUserId = null,
                DstUserId = coins[i].UserId
            };
            
            moves.Add(move);
        }

        await Context.Moves.AddRangeAsync(moves, ct);
        saveResult = await SaveDataAsync(ct);

        if (!saveResult)
        {
            Context.Coins.RemoveRange(coins);
            return false;
        }

        return true;
    }

    public async Task<List<TProjection>> GetUserCoinsAsync<TProjection>(
        Func<Coin, TProjection> projection,
        long userId,
        CancellationToken ct = default)
    {
        return await Context.Coins
            .Where(c => c.UserId == userId)
            .Select(c => projection(c))
            .ToListAsync(ct);
    }

    public async Task<bool> AddMovesAsync(
        long srcUserId,
        long dstUserId,
        long amount = 1,
        CancellationToken ct = default)
    {
        var coins = await GetUserCoinsAsync(c => c, srcUserId, ct);
        if (coins.LongCount() < amount)
        {
            return false;
        }

        for (var i = 0; i < amount; i++)
        {
            var move = new Move
            {
                CoinId = coins[i].Id,
                UnixTimestamp = _dateTimeService.UtcNowOffset.ToUnixTimeMilliseconds(),
                SrcUserId = srcUserId,
                DstUserId = dstUserId
            };
            await Context.Moves.AddAsync(move, ct);

            coins[i].UserId = dstUserId;
            Context.Coins.Update(coins[i]);
        }

        var saveResult = await SaveDataAsync(ct);

        return saveResult;
    }

    public async Task<TProjection?> GetCoinWithLongestHistoryOrDefaultAsync<TProjection>(
        Func<Coin, TProjection> projection,
        CancellationToken ct = default)
    {
        var coins = await Context.Coins
            .Include(c => c.Moves)
            .AsSplitQuery()
            .Select(c => new {
                c.Id,
                MovesCount = c.Moves.LongCount()
            })
            .ToListAsync(ct);
        
        if (coins.LongCount() == 0)
        {
            return default;
        }
        
        var coinIdWithLongestHistory = coins.MaxBy(c => c.MovesCount)!.Id;
        var coin = await Context.Coins
            .Include(c => c.Moves).ThenInclude(m => m.SrcUser)
            .Include(c => c.Moves).ThenInclude(m => m.DstUser)
            .AsSplitQuery()
            .Where(c => c.Id == coinIdWithLongestHistory)
            .Select(c => projection(c))
            .FirstOrDefaultAsync(ct);

        return coin;
    }
}
