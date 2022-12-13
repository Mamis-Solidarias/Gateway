using HotChocolate.Diagnostics;
using MamisSolidarias.Utils.Security;
using StackExchange.Redis;

namespace MamisSolidarias.Gateway.Extensions;

internal static class GraphQlExtensions
{
    public static void AddGraphQl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGraphQlHttpClient(Services.Beneficiaries, configuration["GraphQl:Beneficiaries:Url"]);
        services.AddGraphQlHttpClient(Services.Donors, configuration["GraphQl:Donors:Url"]);
        services.AddGraphQlHttpClient(Services.Users, configuration["GraphQl:Users:Url"]);
        services.AddGraphQlHttpClient(Services.Campaigns, configuration["GraphQl:Campaigns:Url"]);

        var redisConnectionString = $"{configuration["Redis:Host"]}:{configuration["Redis:Port"]}";
        services.AddSingleton(ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddGraphQLServer()
            .AddRemoteSchemasFromRedis(configuration["GraphQl:GlobalSchemaName"] ?? throw new ArgumentException("GraphQl:GlobalSchemaName"),
                t => t.GetRequiredService<ConnectionMultiplexer>())
            .AddInstrumentation(t =>
            {
                t.Scopes = ActivityScopes.All;
                t.IncludeDocument = true;
                t.RequestDetails = RequestDetails.All;
                t.IncludeDataLoaderKeys = true;
            });
    }
    

    public static void UseGraphQl(this WebApplication app)
    {
        app.MapGraphQL("/query");
    }
}