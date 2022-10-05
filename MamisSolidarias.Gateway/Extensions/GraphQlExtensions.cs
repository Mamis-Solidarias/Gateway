using MamisSolidarias.Utils.Security;

namespace MamisSolidarias.Gateway.Extensions;

internal static class GraphQlExtensions
{
    public static void AddGraphQl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGraphQlHttpClient(Services.Beneficiaries, configuration["GraphQl:Beneficiaries:Url"]);
        services.AddGraphQlHttpClient(Services.Donors, configuration["GraphQl:Donors:Url"]);
        
        services.AddGraphQLServer()
            .AddRemoteSchema($"{Services.Beneficiaries}gql")
            .AddRemoteSchema($"{Services.Donors}gql")
            ;
    }

    public static void UseGraphQl(this WebApplication app)
    {
        app.MapGraphQL("/query");
    }
}