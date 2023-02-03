using FastEndpoints;
using MamisSolidarias.Gateway.Services;
using MamisSolidarias.Utils.Security;
using Microsoft.AspNetCore.Authentication;

namespace MamisSolidarias.Gateway.Endpoint.Logout;

internal sealed class Endpoint : EndpointWithoutRequest
{
	private readonly IRolesCache _rolesCache;
	public Endpoint(IRolesCache rolesCache)
	{
		_rolesCache = rolesCache;
	}
	public override void Configure()
    {
        Post("/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
	    var userId = User.GetUserId();

	    if (userId is not null)
		    await _rolesCache.ClearRoles(userId.Value);
	    
        await HttpContext.SignOutAsync();
        await SendOkAsync(cancellation: ct);
    }
}