using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MamisSolidarias.Gateway.Extensions;

internal static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddSource(configuration["OpenTelemetry:Name"])
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(
                            serviceName: configuration["OpenTelemetry:Name"],
                            serviceVersion: configuration["OpenTelemetry:Version"]
                        )
                )
                .AddHttpClientInstrumentation(t=> t.RecordException = true)
                .AddAspNetCoreInstrumentation(t => t.RecordException = true)
                .AddHotChocolateInstrumentation();
            
            if (!env.IsProduction())
            {
                tracerProviderBuilder
                    .AddConsoleExporter()
                    .AddJaegerExporter(t =>
                    {
                        var jaegerHost = configuration["OpenTelemetry:Jaeger:Host"];
                        if (jaegerHost is not null)
                            t.AgentHost = jaegerHost;
                    });
            }
        });
    }
}