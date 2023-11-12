using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JwtAuthDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracija logiranja
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Konfiguracija CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") // Zamijenite sa stvarnim URL-om
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Blok koda za konfiguraciju JWT-a
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtConfig.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSingleton(jwtConfig);

// Dodajte usluge u kontejner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();   

// Registracija LoggingMiddleware
app.UseMiddleware<LoggingMiddleware>();

// Primjena CORS policy
app.UseCors("AllowSpecificOrigin");

// Konfiguracija HTTP request pipeline-a.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    // Proverite da li je pozvan dummy-test endpoint
    if (context.Request.Path.StartsWithSegments("/api/Auth/dummy-test"))
    {
        // Logika za logiranje
        
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Dummy test endpoint called");
    }

    await next.Invoke();
});

app.MapControllers();

app.Run();


