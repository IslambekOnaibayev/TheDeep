using Vogen;

namespace TheDeep.Core.PlayerAggregate.Primitives;

[ValueObject<Guid>(conversions: Conversions.SystemTextJson)]
public readonly partial struct PlayerId
{
  public static PlayerId New() => From(Guid.NewGuid());
}
