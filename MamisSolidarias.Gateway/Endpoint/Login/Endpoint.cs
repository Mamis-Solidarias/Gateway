using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastEndpoints;
using MamisSolidarias.HttpClient.Users.UsersClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MamisSolidarias.Gateway.Endpoint.Login;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly IUsersClient _usersClient;

    public Endpoint(IUsersClient usersClient)
    {
        _usersClient = usersClient;
    }

    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            var a = new UsersClient.SignInRequest(req.Email, req.Password);
            var response = await _usersClient.SignIn(a, ct);

            if (response is null)
            {
                await SendErrorsAsync(401, ct);
                return;
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadJwtToken(response.Jwt);

            var claimsIdentity = new ClaimsIdentity(
                token.Claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            var userId = token.Claims.First(t => t.Type is "Id").Value;
            await SendOkAsync(new Response(int.Parse(userId)), ct);
        }
        catch (HttpRequestException ex)
        {
            HttpContext.Response.StatusCode = (int?)ex.StatusCode ?? 500;
            await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(ex.Message), ct);
        }
    }
}