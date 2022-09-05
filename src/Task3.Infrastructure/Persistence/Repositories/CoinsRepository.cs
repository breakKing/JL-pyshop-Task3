using Task3.Application.Common.Interfaces.Repositories;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Repositories;

public class CoinsRepository : GenericRepository<Coin, long>, ICoinsRepository
{
    protected override Func<Coin, long> GetEntityId => (c => c.Id);

    public CoinsRepository(BillingDbContext context) : base(context)
    {
        
    }
}
