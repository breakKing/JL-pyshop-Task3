namespace Task3.Infrastructure.Persistence.Seeds;

public interface IDataSeed
{
    Task SeedAsync(CancellationToken ct = default);
}
