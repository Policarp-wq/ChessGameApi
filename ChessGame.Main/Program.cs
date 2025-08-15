using ChessGame.Database;
using ChessGame.Main.Abstractions;
using ChessGame.Main.Config;
using ChessGame.Main.Endpoints;
using ChessGame.Main.Handlers;
using ChessGame.Main.Hubs;
using ChessGame.Main.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog(dispose: true);
builder.Services.AddOpenApi();
builder.Services.AddSignalR(opt =>
{
    opt.ClientTimeoutInterval = TimeSpan.FromSeconds(10);
});
builder.Services.AddCors();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAuthorization();
var jwtConfig = new JwtConfig()
{
    Audience = "localhost",
    Issuer = Environment.MachineName,
    SecretKey = EnvProvider.GetEnvVal(EnvProvider.JWT_SECRET),
};
builder.Services.Configure<JwtConfig>(opt =>
{
    opt.Audience = jwtConfig.Audience;
    opt.Issuer = jwtConfig.Issuer;
    opt.SecretKey = jwtConfig.SecretKey;
});
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = jwtConfig.GetSecurityKey(),
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = false,
        };
    });

string? connectionString = Environment.GetEnvironmentVariable(EnvProvider.DB_LINK);
if (string.IsNullOrEmpty(connectionString))
{
    Log.Fatal("Connection link to db {Link} is not provided", EnvProvider.DB_LINK);
    throw new Exception();
}
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(connectionString);
});
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

//app.UseSerilogRequestLogging();
app.UseCors(p =>
{
    p.WithOrigins("http://localhost:5173").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
    ;
});

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithDarkMode().WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

//app.UseHttpsRedirection();

app.MapHub<ChessHub>("/chessHub");
Log.Information("Chess game server is now running");
app.UseUserEndpoints();
app.UseExceptionHandler();
app.Run();
