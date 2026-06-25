using TheDeep.Core.Interfaces;
using TheDeep.Infrastructure;
using TheDeep.Web.Realtime;

namespace TheDeep.Web.Configurations;

public static class ServiceConfigs
{
  public const string SpaCorsPolicy = "spa";

  public static IServiceCollection AddServiceConfigs(
    this IServiceCollection services,
    Microsoft.Extensions.Logging.ILogger logger,
    WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatorSourceGen(logger);

    services.AddSignalR();
    services.AddSingleton<PlayerPresence>();
    services.AddSingleton<IPlayerPresence>(sp => sp.GetRequiredService<PlayerPresence>());
    services.AddSingleton<IOnlinePlayers>(sp => sp.GetRequiredService<PlayerPresence>());
    services.AddSingleton<IGameBroadcaster, GameBroadcaster>();
    services.AddSingleton<DisconnectMonitor>();

    var devOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? ["http://localhost:4200", "https://localhost:4200"];
    services.AddCors(options =>
      options.AddPolicy(SpaCorsPolicy, policy =>
        policy.WithOrigins(devOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

    logger.LogInformation("{Project} services registered", "Infrastructure, Mediator, SignalR");

    return services;
  }
}
