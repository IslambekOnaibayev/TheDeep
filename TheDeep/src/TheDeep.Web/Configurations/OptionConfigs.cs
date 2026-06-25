namespace TheDeep.Web.Configurations;

public static class OptionConfigs
{
  public static IServiceCollection AddOptionConfigs(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    Microsoft.Extensions.Logging.ILogger logger,
                                                    WebApplicationBuilder builder)
  {
    services.Configure<CookiePolicyOptions>(options =>
    {
      options.CheckConsentNeeded = _ => true;
      options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    logger.LogInformation("{Project} were configured", "Options");

    return services;
  }
}
