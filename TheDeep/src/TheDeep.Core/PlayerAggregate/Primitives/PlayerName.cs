using Vogen;

namespace TheDeep.Core.PlayerAggregate.Primitives;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public readonly partial struct PlayerName
{
  public const int MaxLength = 24;

  private static string NormalizeInput(string input) => input.Trim();

  private static Validation Validate(string value)
  {
    var trimmed = value?.Trim() ?? string.Empty;
    if (trimmed.Length == 0) return Validation.Invalid("Name cannot be empty.");
    if (trimmed.Length > MaxLength) return Validation.Invalid($"Name cannot exceed {MaxLength} characters.");
    return Validation.Ok;
  }
}
