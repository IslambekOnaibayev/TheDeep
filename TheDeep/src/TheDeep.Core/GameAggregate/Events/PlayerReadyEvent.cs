namespace TheDeep.Core.GameAggregate.Events;

public sealed class PlayerReadyEvent(GameId gameId, PlayerId playerId) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId PlayerId { get; } = playerId;
}
