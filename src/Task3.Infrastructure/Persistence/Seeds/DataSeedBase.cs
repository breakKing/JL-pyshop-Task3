using Microsoft.Extensions.Logging;

namespace Task3.Infrastructure.Persistence.Seeds;

public abstract class DataSeedBase<TSeed> : IDataSeed
{
    protected ILogger<TSeed> Logger { get; }

    protected DataSeedBase(ILogger<TSeed> logger)
    {
        Logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        try
        {
            await TrySeedAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred during data seed of type {type}", typeof(TSeed));
            throw;
        }
    }

    protected abstract Task TrySeedAsync(CancellationToken ct = default);
}
