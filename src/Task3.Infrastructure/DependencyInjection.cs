using Microsoft.Extensions.DependencyInjection;
using Task3.Infrastructure.Persistence;

namespace Task3.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPersistence();

        return services;
    }
}
