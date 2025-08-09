using ChessGame.Main.Abstractions;
using ChessGame.Main.Hubs;
using ChessGame.Main.Services;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseCors(p =>
{
    p.WithOrigins("http://localhost:5173").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
    ;
});
app.MapHub<ChessHub>("/chessHub");
Log.Information("Chess game server is now running");
app.Run();
Log.CloseAndFlush();
