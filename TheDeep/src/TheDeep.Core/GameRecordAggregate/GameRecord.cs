using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Core.GameRecordAggregate;

public sealed class GameRecord : EntityBase<GameRecord, Guid>, IAggregateRoot
{
  private GameRecord() { }

  public PlayerId WinnerId { get; private set; }
  public PlayerId LoserId { get; private set; }
  public string WinnerName { get; private set; } = string.Empty;
  public string LoserName { get; private set; } = string.Empty;
  public int GridWidth { get; private set; }
  public int GridHeight { get; private set; }
  public bool EndedByForfeit { get; private set; }
  public DateTimeOffset FinishedAt { get; private set; }

  public static GameRecord Create(
    PlayerId winnerId, string winnerName,
    PlayerId loserId, string loserName,
    int gridWidth, int gridHeight, bool endedByForfeit)
  {
    return new GameRecord
    {
      Id = Guid.NewGuid(),
      WinnerId = winnerId,
      WinnerName = winnerName,
      LoserId = loserId,
      LoserName = loserName,
      GridWidth = gridWidth,
      GridHeight = gridHeight,
      EndedByForfeit = endedByForfeit,
      FinishedAt = DateTimeOffset.UtcNow,
    };
  }
}
