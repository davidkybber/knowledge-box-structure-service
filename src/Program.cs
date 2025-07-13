using Microsoft.EntityFrameworkCore;
using KnowledgeBox.Structure.Data;
using KnowledgeBox.Structure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Get CORS origins from environment variable first, then fall back to configuration
        var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
        string[] allowedOrigins;
        
        if (!string.IsNullOrEmpty(corsOriginsEnv))
        {
            allowedOrigins = corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(origin => origin.Trim())
                                          .ToArray();
        }
        else
        {
            allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        }
        
        var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? new[] { "*" };
        var allowCredentials = builder.Configuration.GetValue<bool>("Cors:AllowCredentials");

        if (allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.WithMethods(allowedMethods)
              .WithHeaders(allowedHeaders);

        if (allowCredentials && !allowedOrigins.Contains("*"))
        {
            policy.AllowCredentials();
        }
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "KnowledgeBox API", Version = "v1" });
});

// Add Entity Framework
builder.Services.AddDbContext<KnowledgeBoxContext>(options =>
    options.UseInMemoryDatabase("KnowledgeBoxDb"));

// Add services
builder.Services.AddScoped<IKnowledgeBoxService, KnowledgeBoxService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS middleware
app.UseCors();

app.MapControllers();

app.Run();
