using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Task3.Application.Common.Interfaces;
using Task3.Infrastructure.Persistence.Repositories;

namespace Task3.Infrastructure.Persistence;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<BillingDbContext>(options =>
        {
            options.UseInMemoryDatabase("BillingDb");
        });

        services.AddRepositories();
        
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICoinsRepository, CoinsRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();

        return services;
    }
}
