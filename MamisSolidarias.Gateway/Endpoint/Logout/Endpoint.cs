using FastEndpoints;
using Microsoft.AspNetCore.Authentication;

namespace MamisSolidarias.Gateway.Endpoint.Logout;

internal sealed class Endpoint : EndpointWithoutRequest
{
	public override void Configure()
    {
        Post("/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
	    await HttpContext.SignOutAsync();
        await SendOkAsync(cancellation: ct);
    }
}