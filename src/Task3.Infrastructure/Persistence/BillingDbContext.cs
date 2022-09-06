using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Task3.Domain.Entities;

namespace Task3.Infrastructure.Persistence;

public class BillingDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Coin> Coins => Set<Coin>();
    public DbSet<Move> Moves => Set<Move>();

    public BillingDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
