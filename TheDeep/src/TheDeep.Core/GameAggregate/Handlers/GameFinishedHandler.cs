using TheDeep.Core.GameAggregate.Events;
using TheDeep.Core.GameRecordAggregate;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Core.GameAggregate.Handlers;

public sealed class GameFinishedHandler(
  IRepository<Player> players,
  IRepository<GameRecord> records)
  : INotificationHandler<GameFinishedEvent>
{
  public async ValueTask Handle(GameFinishedEvent notification, CancellationToken cancellationToken)
  {
    var winner = await players.GetByIdAsync(notification.WinnerId, cancellationToken).ConfigureAwait(false);
    if (winner is not null)
    {
      winner.RecordWin();
      await players.UpdateAsync(winner, cancellationToken).ConfigureAwait(false);
    }

    var loser = await players.GetByIdAsync(notification.LoserId, cancellationToken).ConfigureAwait(false);
    if (loser is not null)
    {
      loser.RecordLoss();
      await players.UpdateAsync(loser, cancellationToken).ConfigureAwait(false);
    }

    var record = GameRecord.Create(
      notification.WinnerId, notification.WinnerName.Value,
      notification.LoserId, notification.LoserName.Value,
      notification.GridWidth, notification.GridHeight,
      notification.ByForfeit);

    await records.AddAsync(record, cancellationToken).ConfigureAwait(false);
  }
}
