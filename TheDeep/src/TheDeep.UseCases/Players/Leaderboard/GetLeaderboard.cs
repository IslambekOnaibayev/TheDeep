using TheDeep.Core.PlayerAggregate;
using TheDeep.Core.PlayerAggregate.Specifications;

namespace TheDeep.UseCases.Players.Leaderboard;

public sealed record GetLeaderboardQuery(int Top = 20) : IQuery<Result<IReadOnlyList<LeaderboardEntryDto>>>;

public sealed class GetLeaderboardHandler(IReadRepository<Player> repository)
  : IQueryHandler<GetLeaderboardQuery, Result<IReadOnlyList<LeaderboardEntryDto>>>
{
  public async ValueTask<Result<IReadOnlyList<LeaderboardEntryDto>>> Handle(
    GetLeaderboardQuery query, CancellationToken cancellationToken)
  {
    var top = Math.Clamp(query.Top, 1, 100);
    var players = await repository.ListAsync(new TopPlayersByWinsSpec(top), cancellationToken).ConfigureAwait(false);

    IReadOnlyList<LeaderboardEntryDto> entries =
    [
      .. players.Select((p, i) => new LeaderboardEntryDto(
        i + 1, p.Id.Value, p.Name.Value, p.Wins, p.Losses, p.GamesPlayed, p.WinRate))
    ];
    return Result<IReadOnlyList<LeaderboardEntryDto>>.Success(entries);
  }
}
