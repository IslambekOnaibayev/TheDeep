using Vogen;

namespace TheDeep.Core.GameAggregate.Primitives;

[ValueObject<Guid>(conversions: Conversions.SystemTextJson)]
public readonly partial struct GameId
{
  public static GameId New() => From(Guid.NewGuid());
}
