using TheDeep.Core.GameAggregate;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;

namespace TheDeep.UseCases.Games.Fire;

public sealed record FireShotCommand(
  GameId GameId,
  PlayerId PlayerId,
  Coordinate Target) : ICommand<Result<ShotOutcome>>;

public sealed class FireShotHandler(GameMutationExecutor executor)
  : ICommandHandler<FireShotCommand, Result<ShotOutcome>>
{
  public ValueTask<Result<ShotOutcome>> Handle(FireShotCommand command, CancellationToken cancellationToken) =>
    executor.Execute(command.GameId, game => game.FireShot(command.PlayerId, command.Target));
}
