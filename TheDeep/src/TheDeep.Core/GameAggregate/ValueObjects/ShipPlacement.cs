namespace TheDeep.Core.GameAggregate.ValueObjects;

public sealed record ShipPlacement(Coordinate Bow, Orientation Orientation, int Length)
{
  public IReadOnlyList<Coordinate> Cells()
  {
    var cells = new List<Coordinate>(Length);
    for (var i = 0; i < Length; i++)
    {
      cells.Add(Orientation == Orientation.Horizontal
        ? new Coordinate(Bow.X + i, Bow.Y)
        : new Coordinate(Bow.X, Bow.Y + i));
    }
    return cells;
  }
}
