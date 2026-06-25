using TheDeep.UseCases.Games;

namespace TheDeep.Web.Realtime;

public interface IGameClient
{
  Task ReceiveState(GameStateDto state);
  Task LobbyUpdated(IReadOnlyList<GameSummaryDto> games);
  Task GameError(string message);

  /// <summary>Sent to an older connection when the same identity signs in elsewhere.</summary>
  Task SessionReplaced();

  /// <summary>Sent to the remaining player when the opponent leaves before the battle starts.</summary>
  Task GameCancelled(Guid gameId);
}
