using FastEndpoints;
using FastEndpoints.Swagger;

namespace MamisSolidarias.Gateway.Extensions;

internal static class EndpointsExtensions
{
    public static void AddAuthEndpoints(this IServiceCollection service, IConfiguration configuration, IWebHostEnvironment env)
    {
        service.AddFastEndpoints(t=> t.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);
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