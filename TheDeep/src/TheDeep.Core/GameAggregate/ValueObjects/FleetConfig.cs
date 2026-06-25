namespace TheDeep.Core.GameAggregate.ValueObjects;

public sealed record ShipSpec(int Length, int Count)
{
  public bool IsValid => Length >= 1 && Count >= 1;
}

public sealed record FleetConfig
{
  public IReadOnlyList<ShipSpec> Ships { get; }

  public FleetConfig(IReadOnlyList<ShipSpec> ships)
  {
    if (ships is null || ships.Count == 0)
      throw new ArgumentException("A fleet must contain at least one ship.", nameof(ships));
    if (ships.Any(s => !s.IsValid))
      throw new ArgumentException("Every ship spec must have positive length and count.", nameof(ships));
    Ships = ships.ToArray();
  }

  public int TotalShips => Ships.Sum(s => s.Count);
  public int TotalCells => Ships.Sum(s => s.Length * s.Count);

  public IReadOnlyList<int> ExpandedLengths =>
    Ships.SelectMany(s => Enumerable.Repeat(s.Length, s.Count)).OrderByDescending(l => l).ToArray();

  public static FleetConfig Classic { get; } = new([
    new ShipSpec(4, 1),
    new ShipSpec(3, 2),
    new ShipSpec(2, 3),
    new ShipSpec(1, 4)
  ]);

  public bool FitsOn(GridSize grid) =>
    Ships.All(s => s.Length <= Math.Max(grid.Width, grid.Height)) &&
    TotalCells <= grid.CellCount / 2;
}
