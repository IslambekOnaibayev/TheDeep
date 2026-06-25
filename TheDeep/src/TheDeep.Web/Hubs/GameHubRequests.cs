using TheDeep.Core.GameAggregate;

namespace TheDeep.Web.Hubs;

public sealed record ShipSpecInput(int Length, int Count);

public sealed record CreateGameRequest(
  int Width,
  int Height,
  IReadOnlyList<ShipSpecInput> Fleet,
  bool ShipsMayTouch,
  bool ExtraTurnOnHit,
  string? Title)
{
  public GameSettings ToSettings()
  {
    var fleet = new FleetConfig([.. Fleet.Select(s => new ShipSpec(s.Length, s.Count))]);
    return new GameSettings(new GridSize(Width, Height), fleet, ShipsMayTouch, ExtraTurnOnHit);
  }
}

public sealed record ShipPlacementInput(int X, int Y, string Orientation, int Length)
{
  public ShipPlacement ToPlacement()
  {
    var orientation = Enum.TryParse<TheDeep.Core.GameAggregate.Enums.Orientation>(Orientation, ignoreCase: true, out var o)
      ? o
      : TheDeep.Core.GameAggregate.Enums.Orientation.Horizontal;
    return new ShipPlacement(new Coordinate(X, Y), orientation, Length);
  }
}

public sealed record PlaceShipsRequest(IReadOnlyList<ShipPlacementInput> Ships)
{
  public IReadOnlyList<ShipPlacement> ToPlacements() => [.. Ships.Select(s => s.ToPlacement())];
}
