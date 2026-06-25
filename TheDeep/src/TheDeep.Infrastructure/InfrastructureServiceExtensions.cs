using TheDeep.Core.Interfaces;
using TheDeep.Infrastructure.Data;
using TheDeep.Infrastructure.Games;
using TheDeep.UseCases.Games;

namespace TheDeep.Infrastructure;

public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    var connectionString = PostgresConnectionString.Resolve(config);
    Guard.Against.NullOrEmpty(connectionString, message: "No PostgreSQL connection string was configured.");

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var interceptor = provider.GetRequiredService<EventDispatchInterceptor>();
      options.UseNpgsql(connectionString);
      options.AddInterceptors(interceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

    services.AddSingleton<IGameRegistry, GameRegistry>();
    services.AddScoped<GameMutationExecutor>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
