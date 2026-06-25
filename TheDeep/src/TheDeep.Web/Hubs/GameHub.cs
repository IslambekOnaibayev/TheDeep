using Ardalis.Result;
using Microsoft.AspNetCore.SignalR;
using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;
using TheDeep.UseCases.Games.Create;
using TheDeep.UseCases.Games.Fire;
using TheDeep.UseCases.Games.Join;
using TheDeep.UseCases.Games.Leave;
using TheDeep.UseCases.Games.PlaceShips;
using TheDeep.UseCases.Games.State;
using TheDeep.Web.Realtime;

namespace TheDeep.Web.Hubs;

public sealed class GameHub(
  IMediator mediator,
  IGameRegistry registry,
  IPlayerPresence presence,
  IGameBroadcaster broadcaster,
  DisconnectMonitor disconnectMonitor) : Hub<IGameClient>
{
  public override async Task OnConnectedAsync()
  {
    var http = Context.GetHttpContext();
    var playerIdRaw = http?.Request.Query["playerId"].ToString();
    var name = http?.Request.Query["name"].ToString() ?? string.Empty;

    if (Guid.TryParse(playerIdRaw, out var guid))
    {
      var playerId = PlayerId.From(guid);

      // One live session per identity: kick any older connections of the same
      // player before registering this one, so the same name can't be used twice.
      var previous = presence.ConnectionsOf(playerId);
      presence.Add(Context.ConnectionId, playerId, name);
      if (previous.Count > 0)
        await Clients.Clients(previous).SessionReplaced();
    }

    await Groups.AddToGroupAsync(Context.ConnectionId, GameBroadcaster.LobbyGroup);
    await Clients.Caller.LobbyUpdated([.. registry.OpenGames().Select(GameStateProjector.ToSummary)]);
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var connected = presence.Get(Context.ConnectionId);
    presence.Remove(Context.ConnectionId);
    if (connected is not null)
      disconnectMonitor.ScheduleForfeit(connected.PlayerId);
    await base.OnDisconnectedAsync(exception);
  }

  public async Task<Guid> CreateGame(CreateGameRequest request)
  {
    var (playerId, name) = RequireCaller();
    var result = await mediator.Send(new CreateGameCommand(playerId, PlayerName.From(name), request.ToSettings(), request.Title));
    var gameId = Ensure(result);

    await broadcaster.BroadcastStateAsync(gameId);
    await broadcaster.BroadcastLobbyAsync();
    return gameId.Value;
  }

  public async Task JoinGame(Guid gameId)
  {
    var (playerId, name) = RequireCaller();
    var result = await mediator.Send(new JoinGameCommand(GameId.From(gameId), playerId, PlayerName.From(name)));
    Ensure(result);
    await broadcaster.BroadcastLobbyAsync();
  }

  public async Task PlaceShips(Guid gameId, PlaceShipsRequest request)
  {
    var (playerId, _) = RequireCaller();
    var result = await mediator.Send(new PlaceShipsCommand(GameId.From(gameId), playerId, request.ToPlacements()));
    Ensure(result);
  }

  public async Task FireShot(Guid gameId, int x, int y)
  {
    var (playerId, _) = RequireCaller();
    var result = await mediator.Send(new FireShotCommand(GameId.From(gameId), playerId, new Coordinate(x, y)));
    Ensure(result);
  }

  public async Task LeaveGame(Guid gameId)
  {
    var (playerId, _) = RequireCaller();
    await mediator.Send(new LeaveGameCommand(GameId.From(gameId), playerId));
    await broadcaster.BroadcastStateAsync(GameId.From(gameId));
    await broadcaster.BroadcastLobbyAsync();
  }

  public async Task<GameStateDto> RequestState(Guid gameId)
  {
    var (playerId, _) = RequireCaller();
    var result = await mediator.Send(new GetGameStateQuery(GameId.From(gameId), playerId));
    return Ensure(result);
  }

  public Task RefreshLobby() => broadcaster.BroadcastLobbyAsync();

  private (PlayerId PlayerId, string Name) RequireCaller()
  {
    var connected = presence.Get(Context.ConnectionId)
      ?? throw new HubException("Your session is not registered. Please sign in again.");
    return (connected.PlayerId, connected.Name);
  }

  private static T Ensure<T>(Result<T> result)
  {
    if (result.IsSuccess) return result.Value;
    throw new HubException(Describe(result.Status, result.Errors, result.ValidationErrors));
  }

  private static void Ensure(Result result)
  {
    if (result.IsSuccess) return;
    throw new HubException(Describe(result.Status, result.Errors, result.ValidationErrors));
  }

  private static string Describe(ResultStatus status, IEnumerable<string> errors, IEnumerable<ValidationError> validation)
  {
    var messages = validation.Select(v => v.ErrorMessage).Concat(errors).Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
    if (messages.Length > 0) return string.Join("; ", messages);
    return status switch
    {
      ResultStatus.NotFound => "Game not found.",
      ResultStatus.Forbidden => "You are not allowed to do that.",
      _ => "The action could not be completed."
    };
  }
}
