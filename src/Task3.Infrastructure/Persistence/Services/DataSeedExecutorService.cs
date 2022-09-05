using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task3.Infrastructure.Persistence.Seeds;

namespace Task3.Infrastructure.Persistence.Services;

public class DataSeedExecutorService : IHostedService
{
    private readonly IServiceScopeFactory _factory;

    public DataSeedExecutorService(IServiceScopeFactory factory)
    {
        _factory = factory;
    }
    
    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _factory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeedExecutorService>>();
        var seeds = scope.ServiceProvider.GetServices<IDataSeed>();

        try
        {
            foreach (var seed in seeds)
            {
                await seed.SeedAsync(ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database before seeding");
            throw;
        }
    }

    public Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
