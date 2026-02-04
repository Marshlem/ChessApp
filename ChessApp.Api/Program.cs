using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ChessApp.API.Infrastructure.Security;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using ChessApp.API.Data;
using ChessApp.API.Queries.Openings;
using ChessApp.API.Handlers.Repertoire;
using ChessApp.API.Handlers.Openings;
using ChessApp.API.Handlers.OpeningNodes;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GetCandidateMovesQuery>();
builder.Services.AddScoped<GetOpeningDetailsQuery>();
builder.Services.AddScoped<GetRepertoireTreeQuery>();

builder.Services.AddScoped<OpeningNodeReadRepository>();
builder.Services.AddScoped<OpeningNodeWriteRepository>();

builder.Services.AddScoped<CreateOpeningHandler>();
builder.Services.AddScoped<DeleteOpeningHandler>();
builder.Services.AddScoped<AddMoveHandler>();
builder.Services.AddScoped<DeleteOpeningNodeSubtreeHandler>();

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });

const string CorsPolicy = "AppCors";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p =>
        p.WithOrigins(config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()); // optional
});

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("auth", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 5;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
