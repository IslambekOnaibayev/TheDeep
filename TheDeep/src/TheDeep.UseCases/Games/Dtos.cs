namespace TheDeep.UseCases.Games;

public sealed record CoordinateDto(int X, int Y);

public sealed record ShipDto(IReadOnlyList<CoordinateDto> Cells);

public sealed record CellShotDto(int X, int Y, string Result);

public sealed record ShipSpecDto(int Length, int Count);

public sealed record BoardViewDto(
  int Width,
  int Height,
  IReadOnlyList<ShipDto> Ships,
  IReadOnlyList<CellShotDto> Shots);

public sealed record GameSummaryDto(
  Guid GameId,
  string Title,
  string HostName,
  int Width,
  int Height,
  int ShipCount,
  bool ShipsMayTouch,
  bool ExtraTurnOnHit,
  DateTimeOffset CreatedAt);

public sealed record GameStateDto(
  Guid GameId,
  string Title,
  string Status,
  int Width,
  int Height,
  IReadOnlyList<ShipSpecDto> Fleet,
  bool ShipsMayTouch,
  bool ExtraTurnOnHit,
  Guid YourPlayerId,
  bool YouAreHost,
  string HostName,
  string? OpponentName,
  bool YouReady,
  bool OpponentReady,
  Guid? CurrentTurnPlayerId,
  bool IsYourTurn,
  Guid? WinnerId,
  bool EndedByForfeit,
  BoardViewDto YourBoard,
  BoardViewDto EnemyBoard);
