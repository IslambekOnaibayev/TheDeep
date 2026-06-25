namespace TheDeep.UseCases.Players;

public sealed record PlayerDto(
  Guid Id,
  string Name,
  int GamesPlayed,
  int Wins,
  int Losses,
  double WinRate);

public sealed record LeaderboardEntryDto(
  int Rank,
  Guid Id,
  string Name,
  int Wins,
  int Losses,
  int GamesPlayed,
  double WinRate);
