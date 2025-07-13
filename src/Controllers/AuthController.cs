using Microsoft.AspNetCore.Mvc;
using KnowledgeBox.Structure.Utilities;

namespace KnowledgeBox.Structure.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generate a test JWT token for development and testing purposes
    /// </summary>
    /// <param name="userId">User ID for the token</param>
    /// <param name="email">Email for the token</param>
    /// <returns>JWT token</returns>
    [HttpPost("generate-test-token")]
    public ActionResult<object> GenerateTestToken([FromBody] GenerateTokenRequest request)
    {
        try
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "your-256-bit-secret-key-here-make-it-long-enough-for-security-purposes";
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryInMinutes"] ?? "60");

            var token = JwtTokenHelper.GenerateJwtToken(request.UserId, request.Email, secretKey, expiryMinutes);

            return Ok(new
            {
                token = token,
                userId = request.UserId,
                email = request.Email,
                expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Failed to generate token", details = ex.Message });
        }
    }

    /// <summary>
    /// Generate a default test token
    /// </summary>
    [HttpGet("test-token")]
    public ActionResult<object> GetTestToken()
    {
        try
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "your-256-bit-secret-key-here-make-it-long-enough-for-security-purposes";
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryInMinutes"] ?? "60");

            var token = JwtTokenHelper.GenerateJwtToken("test-user-123", "test@example.com", secretKey, expiryMinutes);

            return Ok(new
            {
                token = token,
                userId = "test-user-123",
                email = "test@example.com",
                expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                usage = "Use this token in Authorization header: Bearer " + token
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Failed to generate token", details = ex.Message });
        }
    }
}

public class GenerateTokenRequest
{
    public string UserId { get; set; } = "test-user-123";
    public string Email { get; set; } = "test@example.com";
}