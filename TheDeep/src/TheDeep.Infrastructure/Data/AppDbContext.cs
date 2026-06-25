using TheDeep.Core.GameRecordAggregate;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Player> Players => Set<Player>();
  public DbSet<GameRecord> GameRecords => Set<GameRecord>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
    SaveChangesAsync().GetAwaiter().GetResult();
}
