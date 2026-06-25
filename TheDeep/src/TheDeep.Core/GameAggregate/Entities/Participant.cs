using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Core.GameAggregate.Entities;

internal sealed class Participant(PlayerId playerId, PlayerName name, Board board)
{
  public PlayerId PlayerId { get; } = playerId;
  public PlayerName Name { get; } = name;
  public Board Board { get; } = board;
  public bool IsReady { get; private set; }

  public void MarkReady() => IsReady = true;
}
