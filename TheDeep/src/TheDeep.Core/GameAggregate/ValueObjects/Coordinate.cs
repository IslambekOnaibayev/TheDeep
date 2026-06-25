namespace TheDeep.Core.GameAggregate.ValueObjects;

public readonly record struct Coordinate(int X, int Y)
{

  public IEnumerable<Coordinate> Neighbors8()
  {
    for (var dy = -1; dy <= 1; dy++)
      for (var dx = -1; dx <= 1; dx++)
        if (dx != 0 || dy != 0)
          yield return new Coordinate(X + dx, Y + dy);
  }

  public override string ToString() => $"({X},{Y})";
}
