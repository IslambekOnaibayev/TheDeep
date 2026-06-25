namespace TheDeep.Infrastructure.Data;

public static class PostgresConnectionString
{
  public static string Resolve(IConfiguration config)
  {
    var databaseUrl = config["DATABASE_URL"] ?? Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(databaseUrl))
      return FromUrl(databaseUrl);

    return config.GetConnectionString("DefaultConnection") ?? string.Empty;
  }

  private static string FromUrl(string url)
  {
    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':', 2);
    var user = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
    var database = uri.AbsolutePath.TrimStart('/');
    var port = uri.Port > 0 ? uri.Port : 5432;

    return $"Host={uri.Host};Port={port};Database={database};Username={user};Password={password};" +
           "SSL Mode=Require;Trust Server Certificate=true";
  }
}
