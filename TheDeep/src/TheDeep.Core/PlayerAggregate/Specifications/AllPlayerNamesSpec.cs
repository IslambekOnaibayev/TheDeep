namespace TheDeep.Core.PlayerAggregate.Specifications;

public sealed class AllPlayerNamesSpec : Specification<Player, PlayerName>
{
  public AllPlayerNamesSpec()
  {
    Query.Select(p => p.Name);
  }
}
