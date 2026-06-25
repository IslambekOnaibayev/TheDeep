using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;

namespace TheDeep.UseCases.Games.Leave;

public sealed record LeaveGameCommand(GameId GameId, PlayerId PlayerId) : ICommand<Result>;

public sealed class LeaveGameHandler(GameMutationExecutor executor, IGameRegistry registry)
  : ICommandHandler<LeaveGameCommand, Result>
{
  public async ValueTask<Result> Handle(LeaveGameCommand command, CancellationToken cancellationToken)
  {
    var result = await executor.Execute(command.GameId, game => game.Forfeit(command.PlayerId));

    var game = registry.Find(command.GameId);
    if (game is { Status: GameStatus.Finished, WinnerId: null })
      registry.Remove(command.GameId);

    return result;
  }
}
