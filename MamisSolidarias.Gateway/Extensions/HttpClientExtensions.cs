using MamisSolidarias.Utils.Security;

namespace MamisSolidarias.Gateway.Extensions;

internal static class HttpClientExtensions
{
    public static void AddGraphQlHttpClient(this IServiceCollection services, Services service,string baseAddress)
    {
        services.AddHttpClient($"{service}gql", (provider, client) =>
        {
            client.BaseAddress = new Uri(baseAddress);
            
            var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (context is null)
                return;
        
            if (context.Request.Headers.TryGetValue("Cookie", out var cookie) && cookie.Any())
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie.First());
            }
        });
    }
}