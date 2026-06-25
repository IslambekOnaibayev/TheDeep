namespace TheDeep.Core.GameAggregate.Events;

public sealed class BattleStartedEvent(GameId gameId, PlayerId firstTurnPlayerId) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId FirstTurnPlayerId { get; } = firstTurnPlayerId;
}
