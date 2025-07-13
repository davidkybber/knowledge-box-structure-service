using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Environment.GetEnvironmentVariable("Jwt__Key") ?? jwtSettings["Key"];
var issuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? jwtSettings["Issuer"];
var audience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? jwtSettings["Audience"];

if (string.IsNullOrEmpty(key))
{
    throw new InvalidOperationException("JWT key is not configured. Please set Jwt__Key in appsettings.json or as an environment variable.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(issuer),
            ValidateAudience = !string.IsNullOrEmpty(audience),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.RequireAuthorization(); // Require valid JWT token

// Add a test endpoint to verify JWT token validation
app.MapGet("/protected", [Authorize] (HttpContext context) =>
{
    var userId = context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("id")?.Value;
    var userName = context.User.FindFirst("name")?.Value ?? context.User.Identity?.Name;
    
    return Results.Ok(new { 
        message = "Successfully authenticated", 
        userId = userId,
        userName = userName,
        claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToList()
    });
})
.WithName("ProtectedEndpoint")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
