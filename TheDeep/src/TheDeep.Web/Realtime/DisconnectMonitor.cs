using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games.Leave;

namespace TheDeep.Web.Realtime;

public sealed class DisconnectMonitor(
  IServiceScopeFactory scopeFactory,
  IGameRegistry registry,
  IPlayerPresence presence,
  IGameBroadcaster broadcaster,
  ILogger<DisconnectMonitor> logger)
{
  public static readonly TimeSpan GracePeriod = TimeSpan.FromSeconds(30);

  public void ScheduleForfeit(PlayerId playerId)
  {
    _ = Task.Run(async () =>
    {
      try
      {
        await Task.Delay(GracePeriod);
        if (presence.IsOnline(playerId)) return;

        foreach (var game in registry.AllGames())
        {
          if (game.Status == GameStatus.Finished || !game.IsParticipant(playerId)) continue;

          using var scope = scopeFactory.CreateScope();
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          await mediator.Send(new LeaveGameCommand(game.Id, playerId));
          await broadcaster.BroadcastStateAsync(game.Id);
          await broadcaster.BroadcastLobbyAsync();
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Disconnect forfeit handling failed for player {PlayerId}", playerId);
      }
    });
  }
}
