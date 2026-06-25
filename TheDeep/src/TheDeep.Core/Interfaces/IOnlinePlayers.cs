namespace TheDeep.Core.Interfaces;

/// <summary>
/// Display names of players currently connected. Used at sign-in to suffix a
/// name only when it clashes with someone who is online right now — so a
/// returning name reclaims its existing record (and stats) when it is free.
/// </summary>
public interface IOnlinePlayers
{
  IReadOnlyCollection<string> DisplayNames();
}
