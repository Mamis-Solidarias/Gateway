using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MamisSolidarias.Gateway.Extensions;

internal static class AuthExtensions
{
    public static void AddAuth(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAuthorization();
        service.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "MamisSolidarias.Auth";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = new TimeSpan(48, 0, 0);
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Cookie.HttpOnly = false;
                // Only use this when the sites are on different domains
                options.Cookie.SameSite = SameSiteMode.None;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"]
                };
            });
    }

    public static void UseAuth(this WebApplication app)
    {
        app.UseCookiePolicy(new CookiePolicyOptions{Secure = CookieSecurePolicy.Always});
        app.UseAuthentication();
        app.UseAuthorization();
    }
}