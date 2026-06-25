using System.Collections.Concurrent;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Web.Realtime;

public sealed record Connected(PlayerId PlayerId, string Name);

public interface IPlayerPresence
{
  void Add(string connectionId, PlayerId playerId, string name);
  void Remove(string connectionId);
  Connected? Get(string connectionId);
  IReadOnlyList<string> ConnectionsOf(PlayerId playerId);
  bool IsOnline(PlayerId playerId);
}

public sealed class PlayerPresence : IPlayerPresence, IOnlinePlayers
{
  private readonly ConcurrentDictionary<string, Connected> _byConnection = new();
  private readonly ConcurrentDictionary<PlayerId, HashSet<string>> _byPlayer = new();

  public void Add(string connectionId, PlayerId playerId, string name)
  {
    _byConnection[connectionId] = new Connected(playerId, name);
    var set = _byPlayer.GetOrAdd(playerId, static _ => []);
    lock (set) set.Add(connectionId);
  }

  public void Remove(string connectionId)
  {
    if (!_byConnection.TryRemove(connectionId, out var connected)) return;
    if (_byPlayer.TryGetValue(connected.PlayerId, out var set))
    {
      lock (set) set.Remove(connectionId);
    }
  }

  public Connected? Get(string connectionId) =>
    _byConnection.TryGetValue(connectionId, out var c) ? c : null;

  public IReadOnlyList<string> ConnectionsOf(PlayerId playerId)
  {
    if (!_byPlayer.TryGetValue(playerId, out var set)) return [];
    lock (set) return [.. set];
  }

  public bool IsOnline(PlayerId playerId) => ConnectionsOf(playerId).Count > 0;

  public IReadOnlyCollection<string> DisplayNames() =>
    [.. _byConnection.Values.Select(c => c.Name).Distinct(StringComparer.OrdinalIgnoreCase)];
}
