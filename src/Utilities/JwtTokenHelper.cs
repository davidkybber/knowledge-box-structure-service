using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KnowledgeBox.Structure.Utilities;

public static class JwtTokenHelper
{
    public static string GenerateJwtToken(string userId, string email, string secretKey, int expiryMinutes = 60)
    {
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim("user_id", userId),
                new Claim("sub", userId)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateTestToken(string userId = "test-user-id", string email = "test@example.com")
    {
        return GenerateJwtToken(userId, email, "your-256-bit-secret-key-here-make-it-long-enough-for-security-purposes");
    }
}