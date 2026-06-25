using TheDeep.Core.GameAggregate;

namespace TheDeep.Core.Interfaces;

public interface IGameRegistry
{
  void Add(Game game);
  Game? Find(GameId id);
  void Remove(GameId id);

  IReadOnlyList<Game> OpenGames();

  IReadOnlyList<Game> AllGames();

  SemaphoreSlim GateFor(GameId id);
}
