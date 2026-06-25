using TheDeep.Core.Interfaces;
using TheDeep.UseCases.Games;

namespace TheDeep.UseCases.Games.List;

public sealed record ListOpenGamesQuery : IQuery<Result<IReadOnlyList<GameSummaryDto>>>;

public sealed class ListOpenGamesHandler(IGameRegistry registry)
  : IQueryHandler<ListOpenGamesQuery, Result<IReadOnlyList<GameSummaryDto>>>
{
  public ValueTask<Result<IReadOnlyList<GameSummaryDto>>> Handle(
    ListOpenGamesQuery query, CancellationToken cancellationToken)
  {
    IReadOnlyList<GameSummaryDto> summaries =
      [.. registry.OpenGames().Select(GameStateProjector.ToSummary)];
    return ValueTask.FromResult(Result<IReadOnlyList<GameSummaryDto>>.Success(summaries));
  }
}
