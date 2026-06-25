using TheDeep.UseCases.Players;
using TheDeep.UseCases.Players.Ensure;

namespace TheDeep.Web.Endpoints.Players;

public sealed class SignInRequest
{
  public Guid? PlayerId { get; set; }
  public string Name { get; set; } = string.Empty;
}

public sealed class SignIn(IMediator mediator) : Endpoint<SignInRequest, PlayerDto>
{
  public override void Configure()
  {
    Post("/players");
    AllowAnonymous();
    Summary(s => s.Summary = "Sign in with a display name (no password).");
  }

  public override async Task HandleAsync(SignInRequest req, CancellationToken ct)
  {
    var result = await mediator.Send(new EnsurePlayerCommand(req.PlayerId, req.Name), ct);

    if (result.IsSuccess)
    {
      await Send.OkAsync(result.Value, ct);
      return;
    }

    foreach (var error in result.ValidationErrors)
      AddError(error.ErrorMessage);
    await Send.ErrorsAsync(400, ct);
  }
}
