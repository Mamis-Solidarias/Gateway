using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

namespace MamisSolidarias.Gateway.Transformers;

internal sealed class Cookie2Jwt: RequestTransform
{
    private readonly IConfiguration _configuration;

    public Cookie2Jwt(IConfiguration configuration) => _configuration = configuration;

    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        var user = context.HttpContext.User;
        if (!(user.Identity?.IsAuthenticated ?? false)) 
            return ValueTask.CompletedTask;
        
        var claims = user.Claims.ToList();
        var nameIdentifierClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim is not null && !user.HasClaim(x => x.Type == ClaimTypes.Name))
        {
            claims.Remove(nameIdentifierClaim);
            claims.Add(new Claim(ClaimTypes.Name, nameIdentifierClaim.Value));
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Audience = context.DestinationPrefix,
            Issuer = _configuration["Jwt:Issuer"],
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        return ValueTask.CompletedTask;
    }
}