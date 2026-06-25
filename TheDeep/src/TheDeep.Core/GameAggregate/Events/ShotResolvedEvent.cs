namespace TheDeep.Core.GameAggregate.Events;

public sealed class ShotResolvedEvent(GameId gameId, PlayerId shooter, ShotOutcome outcome) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId Shooter { get; } = shooter;
  public ShotOutcome Outcome { get; } = outcome;
}
