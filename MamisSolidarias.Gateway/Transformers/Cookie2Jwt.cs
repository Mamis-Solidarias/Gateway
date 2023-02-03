using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using MamisSolidarias.Gateway.Services;
using MamisSolidarias.Utils.Security;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

namespace MamisSolidarias.Gateway.Transformers;

internal sealed class Cookie2Jwt: RequestTransform
{
    private readonly IConfiguration _configuration;
    private readonly IRolesCache _rolesCache;
    public Cookie2Jwt(IConfiguration configuration, IRolesCache rolesCache)
    {
	    _configuration = configuration;
	    _rolesCache = rolesCache;
    }

    public override async ValueTask ApplyAsync(RequestTransformContext context)
    {
        var user = context.HttpContext.User;
        if (!(user.Identity?.IsAuthenticated ?? false)) 
            return;

        if (context.HttpContext.Request.Headers.Authorization.Any())
        {
            context.ProxyRequest.Headers.Add("Authorization", context.HttpContext.Request.Headers.Authorization.First());
            return;
        }
        
        var claims = user.Claims.ToList();
        var nameIdentifierClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim is not null && !user.HasClaim(x => x.Type is ClaimTypes.Name))
        {
            claims.Remove(nameIdentifierClaim);
            claims.Add(new Claim(ClaimTypes.Name, nameIdentifierClaim.Value));
        }

        var userId = user.GetUserId();
        if (userId is not null)
        {
	        var permissions = await _rolesCache.GetRoles(userId.Value);
	        claims.AddRange(permissions.Select(t=> new Claim("permissions", t)));
        }


        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentException("Jwt:Key not found in configuration"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Audience = context.DestinationPrefix,
            Issuer = _configuration["Jwt:Issuer"],
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
    }
}