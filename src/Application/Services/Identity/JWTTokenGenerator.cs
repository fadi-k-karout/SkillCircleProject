using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Identity;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    
    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
     
    }
    
    public string GenerateToken(string userId, string userName, IList<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim("UserId", userId)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.ExpiryMinutes)),
            signingCredentials: signingCredentials
        );

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
        // Use conditional logging based on environment or level

        
   
        return encodedToken;
    }
}