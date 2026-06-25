using TheDeep.UseCases.Games;

namespace TheDeep.Web.Realtime;

public interface IGameClient
{
  Task ReceiveState(GameStateDto state);
  Task LobbyUpdated(IReadOnlyList<GameSummaryDto> games);
  Task GameError(string message);
  Task SessionReplaced();
  Task GameCancelled(Guid gameId);
}
