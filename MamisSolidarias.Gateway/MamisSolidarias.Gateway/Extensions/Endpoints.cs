using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

namespace MamisSolidarias.Gateway.Extensions;

static class Endpoints
{
    public static void AddAuthEndpoints(this IServiceCollection service, IConfiguration configuration, IWebHostEnvironment env)
    {
        service.AddFastEndpoints(t=> t.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);
        service.AddAuthenticationJWTBearer(configuration["JWT:Key"]);
        service.AddAuthorization();
        if (!env.IsProduction())
            service.AddSwaggerDoc(t=> t.Title = "Authentication");
    }

    public static void UseAuthEndpoints(this WebApplication app)
    {
        app.UseDefaultExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseFastEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(t => t.ConfigureDefaults());
        }
    }
}