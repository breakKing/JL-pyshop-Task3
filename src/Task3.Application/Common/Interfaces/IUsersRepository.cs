using Task3.Domain.Entities;

namespace Task3.Application.Common.Interfaces;

public interface IUsersRepository : IGenericRepository<User, long>
{
    
}
