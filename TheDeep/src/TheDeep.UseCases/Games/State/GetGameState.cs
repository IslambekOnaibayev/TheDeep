using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;

namespace TheDeep.UseCases.Games.State;

public sealed record GetGameStateQuery(GameId GameId, PlayerId ViewerId)
  : IQuery<Result<GameStateDto>>;

public sealed class GetGameStateHandler(IGameRegistry registry)
  : IQueryHandler<GetGameStateQuery, Result<GameStateDto>>
{
  public async ValueTask<Result<GameStateDto>> Handle(GetGameStateQuery query, CancellationToken cancellationToken)
  {
    var game = registry.Find(query.GameId);
    if (game is null) return Result<GameStateDto>.NotFound("Game not found.");
    if (!game.IsParticipant(query.ViewerId)) return Result<GameStateDto>.Forbidden();

    var gate = registry.GateFor(query.GameId);
    await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
    try
    {
      return Result<GameStateDto>.Success(GameStateProjector.ToState(game, query.ViewerId));
    }
    finally
    {
      gate.Release();
    }
  }
}
