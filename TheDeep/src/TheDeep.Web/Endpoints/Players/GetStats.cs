using TheDeep.UseCases.Players;
using TheDeep.UseCases.Players.Stats;

namespace TheDeep.Web.Endpoints.Players;

public sealed class GetStatsRequest
{
  public Guid PlayerId { get; set; }
}

public sealed class GetStats(IMediator mediator) : Endpoint<GetStatsRequest, PlayerDto>
{
  public override void Configure()
  {
    Get("/players/{playerId}/stats");
    AllowAnonymous();
    Summary(s => s.Summary = "Get a player's aggregate stats.");
  }

  public override async Task HandleAsync(GetStatsRequest req, CancellationToken ct)
  {
    var result = await mediator.Send(new GetPlayerStatsQuery(req.PlayerId), ct);

    if (result.IsSuccess)
    {
      await Send.OkAsync(result.Value, ct);
      return;
    }

    await Send.NotFoundAsync(ct);
  }
}
