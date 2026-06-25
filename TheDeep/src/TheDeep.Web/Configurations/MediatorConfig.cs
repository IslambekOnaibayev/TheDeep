using Ardalis.SharedKernel;
using TheDeep.Core.GameAggregate;
using TheDeep.Infrastructure;
using TheDeep.UseCases.Games.Create;

namespace TheDeep.Web.Configurations;

public static class MediatorConfig
{

  public static IServiceCollection AddMediatorSourceGen(this IServiceCollection services,
    Microsoft.Extensions.Logging.ILogger logger)
  {
    logger.LogInformation("Registering Mediator SourceGen and Behaviors");
    services.AddMediator(options =>
    {

      options.ServiceLifetime = ServiceLifetime.Scoped;

      options.Assemblies =
      [
        typeof(Game),
        typeof(CreateGameCommand),
        typeof(InfrastructureServiceExtensions),
        typeof(MediatorConfig)
      ];

      options.PipelineBehaviors =
      [
        typeof(LoggingBehavior<,>)
      ];

    });

    return services;
  }
}
