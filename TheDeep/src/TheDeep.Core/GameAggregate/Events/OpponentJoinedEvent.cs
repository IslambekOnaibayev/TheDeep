namespace TheDeep.Core.GameAggregate.Events;

public sealed class OpponentJoinedEvent(GameId gameId, PlayerId playerId, PlayerName name) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId PlayerId { get; } = playerId;
  public PlayerName Name { get; } = name;
}
