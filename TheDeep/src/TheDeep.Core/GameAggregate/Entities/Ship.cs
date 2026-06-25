namespace TheDeep.Core.GameAggregate.Entities;

public sealed class Ship
{
  private readonly HashSet<Coordinate> _cells;
  private readonly HashSet<Coordinate> _hits = [];

  public Ship(IEnumerable<Coordinate> cells)
  {
    _cells = [.. cells];
    if (_cells.Count == 0)
      throw new GameRuleException("A ship must occupy at least one cell.");
  }

  public IReadOnlyCollection<Coordinate> Cells => _cells;
  public int Length => _cells.Count;
  public bool IsSunk => _hits.Count == _cells.Count;

  public bool Occupies(Coordinate c) => _cells.Contains(c);

  public void RegisterHit(Coordinate c)
  {
    if (_cells.Contains(c)) _hits.Add(c);
  }

  public IEnumerable<Coordinate> SurroundingCells() =>
    _cells.SelectMany(c => c.Neighbors8()).Distinct().Where(c => !_cells.Contains(c));
}
