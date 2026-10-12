using System.Text;
using CampusEats.Api.Data;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// PRODUCTION CONFIGURATION (Lab 11)
// Every value below is read from configuration, which on the cloud
// host means an environment variable. Nothing secret is in code.
//   ConnectionStrings__Default  -> the managed PostgreSQL database
//   Jwt__Key / Jwt__Issuer / Jwt__Audience -> token signing
//   FRONTEND_ORIGIN             -> the exact origin allowed by CORS
// ---------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "ConnectionStrings__Default is not configured.");

// Railway/Render inject PORT; ASPNETCORE_URLS must bind 0.0.0.0 so the
// platform's proxy can reach the container (not just localhost).
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(connectionString));

// The data store is now PostgreSQL, so the service is per-request
// (AddScoped) rather than the AddSingleton used in Lab 03.
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException(
                        "Jwt__Key is not configured.")))
        };
    });
builder.Services.AddAuthorization();

// CORS: a browser will only let the deployed frontend call this API if
// the API names that exact origin (https, no trailing slash).
var allowedOrigins = (Environment.GetEnvironmentVariable("FRONTEND_ORIGIN")
        ?? "http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
    p.WithOrigins(allowedOrigins)
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

// Swagger is left on in production so the deployed API can be
// demonstrated and tested from the browser for this lab.
app.UseSwagger();
app.UseSwaggerUI();

// No UseHttpsRedirection here: the platform terminates TLS at its edge
// and forwards plain HTTP to the container, so redirecting would loop.
app.UseCors("Frontend"); // must come before MapControllers()

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Monitoring endpoint: a cheap, dependency-checking URL the platform
// and the logs can be watched against.
app.MapGet("/health", async (AppDbContext db) =>
{
    var dbUp = await db.Database.CanConnectAsync();
    return Results.Json(new
    {
        status = dbUp ? "healthy" : "degraded",
        database = dbUp ? "connected" : "unreachable",
        environment = app.Environment.EnvironmentName,
        utc = DateTime.UtcNow
    }, statusCode: dbUp ? 200 : 503);
});

// Apply any pending EF Core migrations on start-up so the cloud
// database schema always matches the deployed code.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();
    try
    {
        db.Database.Migrate();
        logger.LogInformation(
            "CampusEats API started: migrations applied, listening on port {Port}", port);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed on start-up");
    }
}

app.Run();
