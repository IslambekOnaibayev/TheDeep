using TheDeep.Core.GameAggregate;
using TheDeep.Core.Interfaces;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.UseCases.Games.Create;

public sealed record CreateGameCommand(
  PlayerId HostId,
  PlayerName HostName,
  GameSettings Settings,
  string? Title) : ICommand<Result<GameId>>;

public sealed class CreateGameHandler(IGameRegistry registry) : ICommandHandler<CreateGameCommand, Result<GameId>>
{
  public ValueTask<Result<GameId>> Handle(CreateGameCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var game = Game.Create(command.HostId, command.HostName, command.Settings, command.Title);
      registry.Add(game);
      return ValueTask.FromResult(Result<GameId>.Success(game.Id));
    }
    catch (GameRuleException ex)
    {
      return ValueTask.FromResult(Result<GameId>.Invalid(new ValidationError(ex.Message)));
    }
    catch (ArgumentException ex)
    {
      return ValueTask.FromResult(Result<GameId>.Invalid(new ValidationError(ex.Message)));
    }
  }
}
