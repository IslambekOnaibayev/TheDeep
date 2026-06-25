using TheDeep.Core.GameAggregate.Events;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Core.GameAggregate;

public sealed class Game : EntityBase<Game, GameId>, IAggregateRoot
{
  private Participant _host = default!;
  private Participant? _opponent;

  private Game() { }

  public GridSize GridSize { get; private set; }
  public FleetConfig Fleet { get; private set; } = default!;
  public bool ShipsMayTouch { get; private set; }
  public bool ExtraTurnOnHit { get; private set; }
  public string Title { get; private set; } = string.Empty;

  public GameStatus Status { get; private set; } = GameStatus.WaitingForOpponent;
  public PlayerId? CurrentTurnPlayerId { get; private set; }
  public PlayerId? WinnerId { get; private set; }
  public bool EndedByForfeit { get; private set; }
  public DateTimeOffset CreatedAt { get; private set; }

  public PlayerId HostId => _host.PlayerId;
  public PlayerName HostName => _host.Name;
  public PlayerId? OpponentId => _opponent?.PlayerId;
  public PlayerName? OpponentName => _opponent?.Name;
  public bool HasOpponent => _opponent is not null;

  public static Game Create(PlayerId hostId, PlayerName hostName, GameSettings settings, string? title = null)
  {
    settings.EnsureValid();

    var game = new Game
    {
      Id = GameId.New(),
      GridSize = settings.GridSize,
      Fleet = settings.Fleet,
      ShipsMayTouch = settings.ShipsMayTouch,
      ExtraTurnOnHit = settings.ExtraTurnOnHit,
      Title = string.IsNullOrWhiteSpace(title) ? $"{hostName.Value}'s game" : title.Trim(),
      CreatedAt = DateTimeOffset.UtcNow,
    };
    game._host = new Participant(hostId, hostName, new Board(settings.GridSize));
    return game;
  }

  public void Join(PlayerId playerId, PlayerName name)
  {
    if (Status != GameStatus.WaitingForOpponent)
      throw new GameRuleException("This game is not open to join.");
    if (playerId == _host.PlayerId)
      throw new GameRuleException("You cannot join your own game.");

    _opponent = new Participant(playerId, name, new Board(GridSize));
    Status = GameStatus.PlacingShips;
    RegisterDomainEvent(new OpponentJoinedEvent(Id, playerId, name));
  }

  public void PlaceShips(PlayerId playerId, IReadOnlyList<ShipPlacement> placements)
  {
    if (Status != GameStatus.PlacingShips)
      throw new GameRuleException("Ships can only be placed during the placement phase.");

    var participant = ParticipantOf(playerId);
    if (participant.IsReady)
      throw new GameRuleException("You have already placed your ships.");

    participant.Board.PlaceFleet(placements, Fleet, ShipsMayTouch);
    participant.MarkReady();
    RegisterDomainEvent(new PlayerReadyEvent(Id, playerId));

    if (_host.IsReady && _opponent!.IsReady)
    {
      Status = GameStatus.InProgress;
      CurrentTurnPlayerId = Random.Shared.Next(2) == 0 ? _host.PlayerId : _opponent.PlayerId;
      RegisterDomainEvent(new BattleStartedEvent(Id, CurrentTurnPlayerId.Value));
    }
  }

  public ShotOutcome FireShot(PlayerId playerId, Coordinate target)
  {
    if (Status != GameStatus.InProgress)
      throw new GameRuleException("The game is not in progress.");
    if (CurrentTurnPlayerId != playerId)
      throw new GameRuleException("It is not your turn.");

    var defender = OpponentOf(playerId);
    var outcome = defender.Board.ReceiveShot(target);

    if (defender.Board.AllShipsSunk)
    {
      FinishWith(playerId, defender.PlayerId, byForfeit: false);
    }
    else if (outcome.Result == ShotResult.Miss || !ExtraTurnOnHit)
    {
      CurrentTurnPlayerId = defender.PlayerId;
    }

    outcome = outcome with { NextTurnPlayerId = CurrentTurnPlayerId, IsGameOver = Status == GameStatus.Finished };
    RegisterDomainEvent(new ShotResolvedEvent(Id, playerId, outcome));
    return outcome;
  }

  public void Forfeit(PlayerId playerId)
  {
    if (Status == GameStatus.Finished) return;

    if (Status != GameStatus.InProgress)
    {
      Status = GameStatus.Finished;
      CurrentTurnPlayerId = null;
      RegisterDomainEvent(new GameCancelledEvent(Id, playerId));
      return;
    }

    var winner = OpponentOf(playerId);
    FinishWith(winner.PlayerId, playerId, byForfeit: true);
  }

  public bool IsParticipant(PlayerId playerId) => playerId == _host.PlayerId || playerId == _opponent?.PlayerId;
  public bool BothPlayersReady => _host.IsReady && (_opponent?.IsReady ?? false);
  public bool IsReady(PlayerId playerId) => ParticipantOf(playerId).IsReady;
  public PlayerName NameOf(PlayerId playerId) => ParticipantOf(playerId).Name;
  public PlayerId OpponentIdOf(PlayerId playerId) => OpponentOf(playerId).PlayerId;

  public IReadOnlyCollection<Coordinate> ShotsAgainst(PlayerId playerId) => ParticipantOf(playerId).Board.ShotsReceived;

  public IReadOnlyList<Ship> FleetOf(PlayerId playerId) => ParticipantOf(playerId).Board.Ships;

  private void FinishWith(PlayerId winnerId, PlayerId loserId, bool byForfeit)
  {
    Status = GameStatus.Finished;
    WinnerId = winnerId;
    EndedByForfeit = byForfeit;
    CurrentTurnPlayerId = null;
    RegisterDomainEvent(new GameFinishedEvent(
      Id,
      winnerId, NameOf(winnerId),
      loserId, NameOf(loserId),
      GridSize.Width, GridSize.Height,
      byForfeit));
  }

  private Participant ParticipantOf(PlayerId id) =>
    id == _host.PlayerId ? _host
    : id == _opponent?.PlayerId ? _opponent
    : throw new GameRuleException("You are not part of this game.");

  private Participant OpponentOf(PlayerId id)
  {
    if (_opponent is null) throw new GameRuleException("The game has no opponent yet.");
    return id == _host.PlayerId ? _opponent
      : id == _opponent.PlayerId ? _host
      : throw new GameRuleException("You are not part of this game.");
  }
}
