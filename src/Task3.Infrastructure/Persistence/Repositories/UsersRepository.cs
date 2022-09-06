using Task3.Application.Common.Interfaces.Repositories;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Repositories;

public class UsersRepository : GenericRepository<User, long>, IUsersRepository
{
    protected override Func<User, long> GetEntityId => (u => u.Id);

    public UsersRepository(BillingDbContext context) : base(context)
    {
        
    }
}
