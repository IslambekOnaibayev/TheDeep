using System.Collections.Concurrent;
using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;

namespace TheDeep.Infrastructure.Games;

public sealed class GameRegistry : IGameRegistry
{
  private readonly ConcurrentDictionary<GameId, Game> _games = new();
  private readonly ConcurrentDictionary<GameId, SemaphoreSlim> _gates = new();

  public void Add(Game game) => _games[game.Id] = game;

  public Game? Find(GameId id) => _games.TryGetValue(id, out var game) ? game : null;

  public void Remove(GameId id)
  {
    _games.TryRemove(id, out _);
    if (_gates.TryRemove(id, out var gate)) gate.Dispose();
  }

  public IReadOnlyList<Game> OpenGames() =>
    [.. _games.Values
        .Where(g => g.Status == GameStatus.WaitingForOpponent)
        .OrderByDescending(g => g.CreatedAt)];

  public IReadOnlyList<Game> AllGames() => [.. _games.Values];

  public SemaphoreSlim GateFor(GameId id) => _gates.GetOrAdd(id, static _ => new SemaphoreSlim(1, 1));
}
