using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MYInfo.ApiFunctionnalTests.Tools;

public static class JwtTokenGenerator
{
    public static string GenerateValidToken(IConfiguration configuration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = configuration["JwtTest:Secret"] ?? string.Empty;
        var key = Encoding.UTF8.GetBytes(secret); // same as in test config
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, configuration["JwtTest:UserName"]!),
            new Claim(ClaimTypes.NameIdentifier, configuration["JwtTest:UserId"]!)
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = configuration["JwtTest:Issuer"],
            Audience = configuration["JwtTest:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

