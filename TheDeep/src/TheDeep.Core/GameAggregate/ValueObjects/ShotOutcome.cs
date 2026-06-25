using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Core.GameAggregate.ValueObjects;

public sealed record ShotOutcome(
  Coordinate Target,
  ShotResult Result,
  IReadOnlyList<Coordinate> RevealedAround)
{
  public IReadOnlyList<Coordinate> SunkShipCells { get; init; } = [];
  public PlayerId? NextTurnPlayerId { get; init; }
  public bool IsGameOver { get; init; }
}
