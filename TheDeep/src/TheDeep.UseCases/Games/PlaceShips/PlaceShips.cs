using TheDeep.Core.GameAggregate;
using TheDeep.Core.PlayerAggregate;
using TheDeep.UseCases.Games;

namespace TheDeep.UseCases.Games.PlaceShips;

public sealed record PlaceShipsCommand(
  GameId GameId,
  PlayerId PlayerId,
  IReadOnlyList<ShipPlacement> Placements) : ICommand<Result>;

public sealed class PlaceShipsHandler(GameMutationExecutor executor) : ICommandHandler<PlaceShipsCommand, Result>
{
  public ValueTask<Result> Handle(PlaceShipsCommand command, CancellationToken cancellationToken) =>
    executor.Execute(command.GameId, game => game.PlaceShips(command.PlayerId, command.Placements));
}
