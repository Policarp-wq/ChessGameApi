using ChessGameApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseCors(p =>
{
    p.WithOrigins("http://localhost:5173").AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader(); ;
});
app.MapHub<ChessHub>("/chessHub");
app.Run();
