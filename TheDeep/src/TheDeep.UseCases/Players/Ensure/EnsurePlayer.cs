using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.UseCases.Players.Ensure;

public sealed record EnsurePlayerCommand(Guid? PlayerId, string Name) : ICommand<Result<PlayerDto>>;

public sealed class EnsurePlayerHandler(IRepository<Player> repository, IOnlinePlayers online)
  : ICommandHandler<EnsurePlayerCommand, Result<PlayerDto>>
{
  public async ValueTask<Result<PlayerDto>> Handle(EnsurePlayerCommand command, CancellationToken cancellationToken)
  {
    var raw = command.Name?.Trim() ?? string.Empty;
    if (raw.Length is < 1 or > PlayerName.MaxLength)
      return Result<PlayerDto>.Invalid(new ValidationError($"Please enter a name (1–{PlayerName.MaxLength} characters)."));

    var allPlayers = await repository.ListAsync(cancellationToken).ConfigureAwait(false);

    // Suffix only to avoid clashing with players who are ONLINE right now, so a
    // returning name reclaims its existing record (and stats) when it is free.
    var taken = online.DisplayNames().ToHashSet(StringComparer.OrdinalIgnoreCase);
    if (command.PlayerId is { } id)
    {
      var self = allPlayers.FirstOrDefault(p => p.Id == PlayerId.From(id));
      if (self is not null) taken.Remove(self.Name.Value);
    }

    var display = Disambiguate(raw, taken);
    if (!PlayerName.TryFrom(display, out var name)) PlayerName.TryFrom(raw, out name);

    // Reuse the existing record for this display name so stats are preserved;
    // create a fresh one only when the name has never been used.
    var player = allPlayers.FirstOrDefault(
      p => string.Equals(p.Name.Value, name.Value, StringComparison.OrdinalIgnoreCase));

    if (player is null)
    {
      player = Player.Create(name);
      await repository.AddAsync(player, cancellationToken).ConfigureAwait(false);
    }
    else
    {
      player.Touch();
      await repository.UpdateAsync(player, cancellationToken).ConfigureAwait(false);
    }

    return Result<PlayerDto>.Success(ToDto(player));
  }

  private static string Disambiguate(string baseName, HashSet<string> taken)
  {
    if (!taken.Contains(baseName)) return baseName;
    for (var n = 2; n < 10_000; n++)
    {
      var candidate = $"{baseName} {n}";
      if (candidate.Length <= PlayerName.MaxLength && !taken.Contains(candidate)) return candidate;
    }
    return baseName;
  }

  internal static PlayerDto ToDto(Player p) =>
    new(p.Id.Value, p.Name.Value, p.GamesPlayed, p.Wins, p.Losses, p.WinRate);
}
