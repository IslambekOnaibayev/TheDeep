namespace TheDeep.Core.GameAggregate.Events;

public sealed class GameCancelledEvent(GameId gameId, PlayerId leftBy) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId LeftBy { get; } = leftBy;
}
