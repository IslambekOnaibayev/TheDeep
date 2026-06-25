namespace TheDeep.Core.GameAggregate.ValueObjects;

public sealed record GameSettings(
  GridSize GridSize,
  FleetConfig Fleet,
  bool ShipsMayTouch = false,
  bool ExtraTurnOnHit = true)
{
  public static GameSettings Classic => new(new GridSize(10, 10), FleetConfig.Classic);

  public void EnsureValid()
  {
    if (!Fleet.FitsOn(GridSize))
      throw new GameRuleException("The selected fleet does not fit on the chosen grid.");
  }
}
