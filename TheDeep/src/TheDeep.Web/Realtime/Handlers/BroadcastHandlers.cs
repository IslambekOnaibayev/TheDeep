using TheDeep.Core.GameAggregate.Events;

namespace TheDeep.Web.Realtime.Handlers;

public sealed class OpponentJoinedBroadcastHandler(IGameBroadcaster broadcaster)
  : INotificationHandler<OpponentJoinedEvent>
{
  public async ValueTask Handle(OpponentJoinedEvent n, CancellationToken ct)
  {
    await broadcaster.BroadcastStateAsync(n.GameId);
    await broadcaster.BroadcastLobbyAsync();
  }
}

public sealed class PlayerReadyBroadcastHandler(IGameBroadcaster broadcaster)
  : INotificationHandler<PlayerReadyEvent>
{
  public ValueTask Handle(PlayerReadyEvent n, CancellationToken ct) =>
    new(broadcaster.BroadcastStateAsync(n.GameId));
}

public sealed class BattleStartedBroadcastHandler(IGameBroadcaster broadcaster)
  : INotificationHandler<BattleStartedEvent>
{
  public ValueTask Handle(BattleStartedEvent n, CancellationToken ct) =>
    new(broadcaster.BroadcastStateAsync(n.GameId));
}

public sealed class ShotResolvedBroadcastHandler(IGameBroadcaster broadcaster)
  : INotificationHandler<ShotResolvedEvent>
{
  public ValueTask Handle(ShotResolvedEvent n, CancellationToken ct) =>
    new(broadcaster.BroadcastStateAsync(n.GameId));
}

public sealed class GameFinishedBroadcastHandler(IGameBroadcaster broadcaster)
  : INotificationHandler<GameFinishedEvent>
{
  public async ValueTask Handle(GameFinishedEvent n, CancellationToken ct)
  {
    await broadcaster.BroadcastStateAsync(n.GameId);
    await broadcaster.BroadcastLobbyAsync();
  }
}

public sealed class GameCancelledBroadcastHandler(IGameBroadcaster broadcaster)
  : INotificationHandler<GameCancelledEvent>
{
  public async ValueTask Handle(GameCancelledEvent n, CancellationToken ct)
  {
    await broadcaster.NotifyCancelledAsync(n.GameId, n.LeftBy);
    await broadcaster.BroadcastLobbyAsync();
  }
}
