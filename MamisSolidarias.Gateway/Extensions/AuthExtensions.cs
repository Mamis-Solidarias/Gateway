using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MamisSolidarias.Gateway.Extensions;

internal static class AuthExtensions
{
    public static void AddAuth(this IServiceCollection service, IConfiguration configuration)
    {
        var options = configuration.GetSection("Jwt").Get<JwtOptions>();

        if (options is null)
        {
            throw new ArgumentException("JWT options not found");
        }
        
        service.AddAuthorization();
        service.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.Cookie.Name = "MamisSolidarias.Auth";
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = new TimeSpan(48, 0, 0);
                o.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                o.Cookie.HttpOnly = false;
                // Only use this when the sites are on different domains
                o.Cookie.SameSite = SameSiteMode.None;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer
                };
            });
    }
    private sealed record JwtOptions(string Key, string Issuer);

    public static void UseAuth(this WebApplication app)
    {
        app.UseCookiePolicy(new CookiePolicyOptions{Secure = CookieSecurePolicy.Always});
        app.UseAuthentication();
        app.UseAuthorization();
    }
}