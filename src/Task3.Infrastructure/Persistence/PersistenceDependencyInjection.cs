using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Task3.Application.Common.Interfaces.Repositories;
using Task3.Infrastructure.Persistence.Repositories;
using Task3.Infrastructure.Persistence.Seeds;
using Task3.Infrastructure.Persistence.Services;

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
        services.AddDataSeeds();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICoinsRepository, CoinsRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();

        return services;
    }

    private static IServiceCollection AddDataSeeds(this IServiceCollection services)
    {
        services.AddScoped<IDataSeed, DefaultUsersDataSeed>();

        services.AddHostedService<DataSeedExecutorService>();

        return services;
    }
}
