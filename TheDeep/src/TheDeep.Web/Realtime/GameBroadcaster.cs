using Microsoft.AspNetCore.SignalR;
using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;
using TheDeep.Web.Hubs;

namespace TheDeep.Web.Realtime;

public interface IGameBroadcaster
{
  Task BroadcastStateAsync(GameId gameId);
  Task BroadcastLobbyAsync();
  Task NotifyCancelledAsync(GameId gameId, PlayerId leftBy);
}

public sealed class GameBroadcaster(
  IHubContext<GameHub, IGameClient> hub,
  IGameRegistry registry,
  IPlayerPresence presence) : IGameBroadcaster
{
  public const string LobbyGroup = "lobby";

  public async Task BroadcastStateAsync(GameId gameId)
  {
    var game = registry.Find(gameId);
    if (game is null) return;

    await SendStateTo(game, game.HostId);
    if (game.OpponentId is { } opponentId)
      await SendStateTo(game, opponentId);
  }

  public async Task BroadcastLobbyAsync()
  {
    IReadOnlyList<GameSummaryDto> games = [.. registry.OpenGames().Select(GameStateProjector.ToSummary)];
    await hub.Clients.Group(LobbyGroup).LobbyUpdated(games);
  }

  public async Task NotifyCancelledAsync(GameId gameId, PlayerId leftBy)
  {
    var game = registry.Find(gameId);
    if (game is null) return;

    var participants = new List<PlayerId> { game.HostId };
    if (game.OpponentId is { } opponentId) participants.Add(opponentId);

    foreach (var playerId in participants)
    {
      if (playerId == leftBy) continue;
      var connections = presence.ConnectionsOf(playerId);
      if (connections.Count > 0)
        await hub.Clients.Clients(connections).GameCancelled(gameId.Value);
    }
  }

  private async Task SendStateTo(Game game, PlayerId playerId)
  {
    var connections = presence.ConnectionsOf(playerId);
    if (connections.Count == 0) return;

    var state = GameStateProjector.ToState(game, playerId);
    await hub.Clients.Clients(connections).ReceiveState(state);
  }
}
