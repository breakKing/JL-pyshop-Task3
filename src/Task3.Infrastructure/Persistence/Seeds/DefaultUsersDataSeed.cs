using Microsoft.Extensions.Logging;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence.Seeds;

public class DefaultUsersDataSeed : DataSeedBase<DefaultUsersDataSeed>
{
    private readonly User[] _defaultUsers = new User[]
    {
        new User
        {
            Name = "boris",
            Rating = 5000
        },
        new User
        {
            Name = "maria",
            Rating = 1000
        },
        new User
        {
            Name = "oleg",
            Rating = 800
        }
    };

    private readonly IUsersRepository _repository;

    public DefaultUsersDataSeed(ILogger<DefaultUsersDataSeed> logger,
        IUsersRepository repository)
            : base(logger)
    {
        _repository = repository;
    }

    protected override async Task TrySeedAsync(CancellationToken ct = default)
    {
        for (int i = 0; i < _defaultUsers.Length; i++)
        {
            await _repository.AddAsync(_defaultUsers[i]);
        }
    }
}
