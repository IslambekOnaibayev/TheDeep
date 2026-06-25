namespace TheDeep.Core.GameAggregate.Events;

public sealed class GameFinishedEvent(
  GameId gameId,
  PlayerId winnerId, PlayerName winnerName,
  PlayerId loserId, PlayerName loserName,
  int gridWidth, int gridHeight,
  bool byForfeit = false) : DomainEventBase
{
  public GameId GameId { get; } = gameId;
  public PlayerId WinnerId { get; } = winnerId;
  public PlayerName WinnerName { get; } = winnerName;
  public PlayerId LoserId { get; } = loserId;
  public PlayerName LoserName { get; } = loserName;
  public int GridWidth { get; } = gridWidth;
  public int GridHeight { get; } = gridHeight;
  public bool ByForfeit { get; } = byForfeit;
}
