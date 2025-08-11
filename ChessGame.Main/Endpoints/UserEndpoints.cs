using System;
using ChessGame.Database.Models;
using ChessGame.Main.Abstractions;
using ChessGame.Main.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ChessGame.Main.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder UseUserEndpoints(this IEndpointRouteBuilder router)
    {
        var group = router.MapGroup("/users");
        group.MapGet("list", GetAll);
        group.MapPost("register", RegisterUser);
        group.MapPost("login", LoginUser);

        return router;
    }

    public static async Task<Created<UserRequestData>> RegisterUser(
        HttpContext context,
        IUserService userService,
        [FromBody] UserInfo userInfo
    )
    {
        return TypedResults.Created(context.Request.Path, await userService.RegisterUser(userInfo));
    }

    public static async Task<Ok<UserRequestData>> LoginUser(
        IUserService userService,
        [FromBody] UserInfo userInfo
    )
    {
        return TypedResults.Ok(await userService.LoginUser(userInfo));
    }

    public static async Task<Ok<List<User>>> GetAll(IUserService userService)
    {
        return TypedResults.Ok(await userService.GetAll());
    }
}
