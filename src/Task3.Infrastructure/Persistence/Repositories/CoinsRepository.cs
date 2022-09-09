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

    public override async Task<long> AddAsync(Coin coin, CancellationToken ct = default)
    {
        var createdCoinId = await base.AddAsync(coin, ct);

        if (createdCoinId is default(long))
        {
            return default;
        }

        var move = new Move
        {
            CoinId = createdCoinId,
            UnixTimestamp = _dateTimeService.UtcNowOffset.ToUnixTimeMilliseconds(),
            SrcUserId = null,
            DstUserId = coin.UserId
        };

        await Context.Moves.AddAsync(move, ct);

        var saveResult = await SaveDataAsync(ct);

        if (saveResult)
        {
            return createdCoinId;
        }

        await RemoveAsync(createdCoinId, ct);
        return default;
    }
}
