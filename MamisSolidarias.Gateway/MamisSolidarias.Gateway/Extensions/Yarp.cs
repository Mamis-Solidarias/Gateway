using MamisSolidarias.Gateway.Transformers;
using Yarp.ReverseProxy.Transforms;

namespace MamisSolidarias.Gateway.Extensions;

public static class Yarp
{
    public static void AddYarp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddTransforms(t =>
            {
                t.RequestTransforms.Add(new Cookie2Jwt(configuration));
                t.RequestTransforms.Add(new RequestHeaderRemoveTransform("Cookie"));
            });
    }

    public static void UseYarp(this WebApplication app)
    {
        app.MapReverseProxy();
    }
}