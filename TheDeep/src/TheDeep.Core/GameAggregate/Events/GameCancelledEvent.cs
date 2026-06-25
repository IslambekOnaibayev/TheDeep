namespace TheDeep.Core.GameAggregate.Events;

/// <summary>
/// Raised when a player leaves before the battle starts (waiting for an opponent
/// or still placing ships). The game is voided — no winner, no stats, no record.
/// </summary>
public sealed class GameCancelledEvent(GameId gameId, PlayerId leftBy) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId LeftBy { get; } = leftBy;
}
