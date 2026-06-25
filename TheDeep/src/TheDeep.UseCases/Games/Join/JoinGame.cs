using TheDeep.Core.GameAggregate;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;

namespace TheDeep.UseCases.Games.Join;

public sealed record JoinGameCommand(GameId GameId, PlayerId PlayerId, PlayerName Name) : ICommand<Result>;

public sealed class JoinGameHandler(GameMutationExecutor executor) : ICommandHandler<JoinGameCommand, Result>
{
  public ValueTask<Result> Handle(JoinGameCommand command, CancellationToken cancellationToken) =>
    executor.Execute(command.GameId, game => game.Join(command.PlayerId, command.Name));
}
