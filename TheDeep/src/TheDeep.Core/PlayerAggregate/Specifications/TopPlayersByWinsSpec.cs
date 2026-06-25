namespace TheDeep.Core.PlayerAggregate.Specifications;

public sealed class TopPlayersByWinsSpec : Specification<Player>
{
  public TopPlayersByWinsSpec(int top)
  {
    Query
      .Where(p => p.GamesPlayed > 0)
      .OrderByDescending(p => p.Wins)
      .ThenBy(p => p.GamesPlayed)
      .Take(top);
  }
}
