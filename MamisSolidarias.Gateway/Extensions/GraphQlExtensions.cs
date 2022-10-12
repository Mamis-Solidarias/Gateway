using HotChocolate.Diagnostics;
using HotChocolate.Execution.Configuration;
using MamisSolidarias.Utils.Security;

namespace MamisSolidarias.Gateway.Extensions;

internal static class GraphQlExtensions
{
    public static void AddGraphQl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGraphQlHttpClient(Services.Beneficiaries, configuration["GraphQl:Beneficiaries:Url"]);
        services.AddGraphQlHttpClient(Services.Donors, configuration["GraphQl:Donors:Url"]);
        services.AddGraphQlHttpClient(Services.Users, configuration["GraphQl:Users:Url"]);
        services.AddGraphQlHttpClient(Services.Campaigns, configuration["GraphQl:Campaigns:Url"]);

        services.AddGraphQLServer()
            .AddGraphQlSchema(services, Services.Beneficiaries, configuration["GraphQl:Beneficiaries:Url"])
            .AddGraphQlSchema(services, Services.Donors, configuration["GraphQl:Donors:Url"])
            .AddGraphQlSchema(services, Services.Users, configuration["GraphQl:Users:Url"])
            .AddGraphQlSchema(services, Services.Campaigns, configuration["GraphQl:Campaigns:Url"])
            .AddInstrumentation(t =>
            {
                t.Scopes = ActivityScopes.All;
                t.IncludeDocument = true;
                t.RequestDetails = RequestDetails.All;
                t.IncludeDataLoaderKeys = true;
            });
    }

    private static IRequestExecutorBuilder AddGraphQlSchema(this IRequestExecutorBuilder graphql, IServiceCollection services , Services name, string? url)
    {
        if ( string.IsNullOrEmpty(url))
        {
            Console.WriteLine($"Skipping {name} because it is not configured");
            return graphql;
        }
        
        Console.WriteLine($"Adding GraphQL Schema for {name} at {url}");

        services.AddGraphQlHttpClient(name, url);
        return graphql.AddRemoteSchema($"{name}gql");
    }

    public static void UseGraphQl(this WebApplication app)
    {
        app.MapGraphQL("/query");
    }
}