using TheDeep.Core.GameAggregate;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.UseCases.Games;

public static class GameStateProjector
{
  public static GameSummaryDto ToSummary(Game game) => new(
    game.Id.Value,
    game.Title,
    game.HostName.Value,
    game.GridSize.Width,
    game.GridSize.Height,
    game.Fleet.TotalShips,
    game.ShipsMayTouch,
    game.ExtraTurnOnHit,
    game.CreatedAt);

  public static GameStateDto ToState(Game game, PlayerId viewerId)
  {
    var youAreHost = viewerId == game.HostId;
    var hasOpponent = game.HasOpponent;
    var enemyId = hasOpponent ? game.OpponentIdOf(viewerId) : (PlayerId?)null;

    var yourBoard = OwnBoard(game, viewerId);
    var enemyBoard = enemyId is { } eid ? EnemyBoard(game, eid) : EmptyBoard(game.GridSize);

    var opponentName = youAreHost ? game.OpponentName?.Value : game.HostName.Value;
    var opponentReady = enemyId is { } oid && game.IsReady(oid);

    return new GameStateDto(
      game.Id.Value,
      game.Title,
      game.Status.ToString(),
      game.GridSize.Width,
      game.GridSize.Height,
      [.. game.Fleet.Ships.Select(s => new ShipSpecDto(s.Length, s.Count))],
      game.ShipsMayTouch,
      game.ExtraTurnOnHit,
      viewerId.Value,
      youAreHost,
      game.HostName.Value,
      opponentName,
      game.IsReady(viewerId),
      opponentReady,
      game.CurrentTurnPlayerId?.Value,
      game.CurrentTurnPlayerId == viewerId,
      game.WinnerId?.Value,
      game.EndedByForfeit,
      yourBoard,
      enemyBoard);
  }

  private static BoardViewDto OwnBoard(Game game, PlayerId ownerId)
  {
    var ships = game.FleetOf(ownerId).Select(ToShipDto).ToArray();
    var shots = ShotsOn(game, ownerId).ToArray();
    return new BoardViewDto(game.GridSize.Width, game.GridSize.Height, ships, shots);
  }

  private static BoardViewDto EnemyBoard(Game game, PlayerId enemyId)
  {
    var sunkShips = game.FleetOf(enemyId).Where(s => s.IsSunk).Select(ToShipDto).ToArray();
    var shots = ShotsOn(game, enemyId).ToArray();
    return new BoardViewDto(game.GridSize.Width, game.GridSize.Height, sunkShips, shots);
  }

  private static BoardViewDto EmptyBoard(GridSize size) =>
    new(size.Width, size.Height, [], []);

  private static IEnumerable<CellShotDto> ShotsOn(Game game, PlayerId boardOwner)
  {
    var ships = game.FleetOf(boardOwner);
    foreach (var c in game.ShotsAgainst(boardOwner))
    {
      var ship = ships.FirstOrDefault(s => s.Occupies(c));
      var result = ship is null ? ShotResult.Miss : ship.IsSunk ? ShotResult.Sunk : ShotResult.Hit;
      yield return new CellShotDto(c.X, c.Y, result.ToString());
    }
  }

  private static ShipDto ToShipDto(Ship ship) =>
    new([.. ship.Cells.Select(c => new CoordinateDto(c.X, c.Y))]);
}
