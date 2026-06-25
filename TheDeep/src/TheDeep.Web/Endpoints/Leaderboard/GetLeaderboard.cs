using TheDeep.UseCases.Players;
using TheDeep.UseCases.Players.Leaderboard;

namespace TheDeep.Web.Endpoints.Leaderboard;

public sealed class GetLeaderboardRequest
{
  public int Top { get; set; } = 20;
}

public sealed class GetLeaderboard(IMediator mediator)
  : Endpoint<GetLeaderboardRequest, IReadOnlyList<LeaderboardEntryDto>>
{
  public override void Configure()
  {
    Get("/leaderboard");
    AllowAnonymous();
    Summary(s => s.Summary = "Top players ranked by wins.");
  }

  public override async Task HandleAsync(GetLeaderboardRequest req, CancellationToken ct)
  {
    var result = await mediator.Send(new GetLeaderboardQuery(req.Top), ct);
    await Send.OkAsync(result.Value, ct);
  }
}
