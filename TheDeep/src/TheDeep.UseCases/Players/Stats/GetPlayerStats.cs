using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Players.Ensure;

namespace TheDeep.UseCases.Players.Stats;

public sealed record GetPlayerStatsQuery(Guid PlayerId) : IQuery<Result<PlayerDto>>;

public sealed class GetPlayerStatsHandler(IReadRepository<Player> repository)
  : IQueryHandler<GetPlayerStatsQuery, Result<PlayerDto>>
{
  public async ValueTask<Result<PlayerDto>> Handle(GetPlayerStatsQuery query, CancellationToken cancellationToken)
  {
    var player = await repository.GetByIdAsync(PlayerId.From(query.PlayerId), cancellationToken).ConfigureAwait(false);
    return player is null
      ? Result<PlayerDto>.NotFound("Player not found.")
      : Result<PlayerDto>.Success(EnsurePlayerHandler.ToDto(player));
  }
}
