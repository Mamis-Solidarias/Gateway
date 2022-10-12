using System.Net;
using MamisSolidarias.Utils.Security;

namespace MamisSolidarias.Gateway.Extensions;

internal static class HttpClientExtensions
{
    public static void AddGraphQlHttpClient(this IServiceCollection services, Services service,string? baseAddress)
    {
        if (baseAddress is null)
            return;
        
        services.AddHttpClient($"{service}gql", (provider, client) =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestVersion = HttpVersion.Version20;
            var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (context is null)
                return;
        
            if (context.Request.Headers.TryGetValue("Cookie", out var cookie) && cookie.Any())
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie.First());
            }
            
            if (context.Request.Headers.TryGetValue("Authorization", out var auth) && auth.Any())
            {
                client.DefaultRequestHeaders.Authorization = null;
                client.DefaultRequestHeaders.Add("Authorization", auth.First());
            }
        });
    }
}