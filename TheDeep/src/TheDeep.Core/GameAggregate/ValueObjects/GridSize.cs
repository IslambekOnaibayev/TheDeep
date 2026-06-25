namespace TheDeep.Core.GameAggregate.ValueObjects;

public readonly record struct GridSize
{
  public const int MinSide = 7;
  public const int MaxSide = 12;

  public int Width { get; }
  public int Height { get; }

  public GridSize(int width, int height)
  {
    if (width is < MinSide or > MaxSide)
      throw new ArgumentOutOfRangeException(nameof(width), width, $"Grid width must be {MinSide}–{MaxSide}.");
    if (height is < MinSide or > MaxSide)
      throw new ArgumentOutOfRangeException(nameof(height), height, $"Grid height must be {MinSide}–{MaxSide}.");
    Width = width;
    Height = height;
  }

  public int CellCount => Width * Height;

  public bool Contains(Coordinate c) => c.X >= 0 && c.Y >= 0 && c.X < Width && c.Y < Height;
}
