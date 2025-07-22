
using ChessGameApi.Models;
using ChessGameApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ChessGameApi.Endpoints
{
    public static class GameEndpoints
    {
        public static IEndpointRouteBuilder UseGameEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("game");
            group.MapPost("create", CreateGame);
            return group;
        }

        private static Ok<Guid> CreateGame(IGameService gameService, [FromBody] User user)
        {
            return TypedResults.Ok(gameService.CreateGameRequest(user));
        }
    }
}
