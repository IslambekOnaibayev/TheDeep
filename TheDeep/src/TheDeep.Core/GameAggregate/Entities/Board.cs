namespace TheDeep.Core.GameAggregate.Entities;

public sealed class Board(GridSize size)
{
  private readonly List<Ship> _ships = [];
  private readonly HashSet<Coordinate> _shotsReceived = [];

  public GridSize Size { get; } = size;
  public bool HasFleet => _ships.Count > 0;
  public bool AllShipsSunk => _ships.Count > 0 && _ships.All(s => s.IsSunk);
  public IReadOnlyList<Ship> Ships => _ships;
  public IReadOnlyCollection<Coordinate> ShotsReceived => _shotsReceived;

  public void PlaceFleet(IReadOnlyList<ShipPlacement> placements, FleetConfig fleet, bool shipsMayTouch)
  {
    if (HasFleet)
      throw new GameRuleException("Ships have already been placed.");

    ValidateFleetComposition(placements, fleet);

    var occupied = new HashSet<Coordinate>();
    var ships = new List<Ship>(placements.Count);

    foreach (var placement in placements)
    {
      var cells = placement.Cells();

      foreach (var cell in cells)
      {
        if (!Size.Contains(cell))
          throw new GameRuleException($"Ship at {placement.Bow} extends outside the grid.");
        if (occupied.Contains(cell))
          throw new GameRuleException($"Ships overlap at {cell}.");
      }

      if (!shipsMayTouch && cells.Any(c => c.Neighbors8().Any(occupied.Contains)))
        throw new GameRuleException("Ships may not touch each other.");

      foreach (var cell in cells) occupied.Add(cell);
      ships.Add(new Ship(cells));
    }

    _ships.AddRange(ships);
  }

  public ShotOutcome ReceiveShot(Coordinate target)
  {
    if (!Size.Contains(target))
      throw new GameRuleException("Target is outside the grid.");
    if (!_shotsReceived.Add(target))
      throw new GameRuleException("That cell has already been targeted.");

    var ship = _ships.FirstOrDefault(s => s.Occupies(target));
    if (ship is null)
      return new ShotOutcome(target, ShotResult.Miss, RevealedAround: []);

    ship.RegisterHit(target);
    if (!ship.IsSunk)
      return new ShotOutcome(target, ShotResult.Hit, RevealedAround: []);

    var around = ship.SurroundingCells().Where(Size.Contains).ToArray();
    foreach (var c in around) _shotsReceived.Add(c);

    return new ShotOutcome(target, ShotResult.Sunk, RevealedAround: around)
    {
      SunkShipCells = ship.Cells.ToArray()
    };
  }

  private static void ValidateFleetComposition(IReadOnlyList<ShipPlacement> placements, FleetConfig fleet)
  {
    if (placements is null || placements.Count == 0)
      throw new GameRuleException("No ships were provided.");

    var actual = placements.Select(p => p.Length).OrderByDescending(l => l).ToArray();
    var expected = fleet.ExpandedLengths;

    if (!actual.SequenceEqual(expected))
      throw new GameRuleException("The placed fleet does not match the required ship configuration.");
  }
}
