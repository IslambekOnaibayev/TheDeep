namespace TheDeep.Core.PlayerAggregate;

public sealed class Player : EntityBase<Player, PlayerId>, IAggregateRoot
{
  private Player() { }

  public PlayerName Name { get; private set; }
  public int GamesPlayed { get; private set; }
  public int Wins { get; private set; }
  public int Losses { get; private set; }
  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset LastSeenAt { get; private set; }

  public double WinRate => GamesPlayed == 0 ? 0 : (double)Wins / GamesPlayed;

  public static Player Create(PlayerName name)
  {
    var now = DateTimeOffset.UtcNow;
    return new Player
    {
      Id = PlayerId.New(),
      Name = name,
      CreatedAt = now,
      LastSeenAt = now,
    };
  }

  public void Rename(PlayerName name)
  {
    Name = name;
    Touch();
  }

  public void RecordWin()
  {
    GamesPlayed++;
    Wins++;
    Touch();
  }

  public void RecordLoss()
  {
    GamesPlayed++;
    Losses++;
    Touch();
  }

  public void Touch() => LastSeenAt = DateTimeOffset.UtcNow;
}
