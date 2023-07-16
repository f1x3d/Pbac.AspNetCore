using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Pbac.AspNetCore.Example;

public static class DummyJwtGenerator
{
    public const string JwtSecret = "dummy-jwt-secret";

    public static string Generate(Claim[] claims)
    {
        var jwtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var jwtIssuer = "http://localhost";
        var jwtAudience = "http://localhost";

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddYears(1000),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(jwtSecurityKey, SecurityAlgorithms.HmacSha256Signature),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
