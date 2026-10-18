using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeeMart.Api.Data.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BeeMart.Api.Security;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key belum diisi.");
        var issuer = _configuration["Jwt:Issuer"] ?? "BeeMart.Api";
        var audience = _configuration["Jwt:Audience"] ?? "BeeMart.WebClient";
        var hours = int.TryParse(_configuration["Jwt:ExpiryHours"], out var parsed) ? parsed : 8;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.IdUser),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new("fullName", user.FullName),
            new(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(hours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
