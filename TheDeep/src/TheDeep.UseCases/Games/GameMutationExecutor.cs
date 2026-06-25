using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;

namespace TheDeep.UseCases.Games;

public sealed class GameMutationExecutor(IGameRegistry registry, IDomainEventDispatcher dispatcher)
{
  public async ValueTask<Result> Execute(GameId id, Action<Game> mutate)
  {
    var r = await Execute(id, game => { mutate(game); return 0; }).ConfigureAwait(false);
    return r.Status switch
    {
      ResultStatus.Ok => Result.Success(),
      ResultStatus.NotFound => Result.NotFound([.. r.Errors]),
      ResultStatus.Invalid => Result.Invalid([.. r.ValidationErrors]),
      _ => Result.Error(string.Join("; ", r.Errors))
    };
  }

  public async ValueTask<Result<T>> Execute<T>(GameId id, Func<Game, T> mutate)
  {
    var game = registry.Find(id);
    if (game is null) return Result<T>.NotFound("Game not found.");

    var gate = registry.GateFor(id);
    await gate.WaitAsync().ConfigureAwait(false);
    try
    {
      T value;
      try
      {
        value = mutate(game);
      }
      catch (GameRuleException ex)
      {
        return Result<T>.Invalid(new ValidationError(ex.Message));
      }
      catch (ArgumentException ex)
      {
        return Result<T>.Invalid(new ValidationError(ex.Message));
      }

      await dispatcher.DispatchAndClearEvents([game]).ConfigureAwait(false);
      return Result<T>.Success(value);
    }
    finally
    {
      gate.Release();
    }
  }
}
