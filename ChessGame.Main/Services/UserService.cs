using System;
using ChessGame.Database;
using ChessGame.Database.Models;
using ChessGame.Main.Abstractions;
using ChessGame.Main.DTOs;
using ChessGame.Main.Exceptions;
using ChessGame.Main.Exceptions.ResponseExceptions;
using ChessGame.Main.Handlers;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Main.Services;

public class UserService(
    AppDbContext _context,
    ILogger<UserService> _logger,
    IJwtService _jwtService
) : IUserService
{
    public async Task<List<User>> GetAll() => await _context.Users.AsNoTracking().ToListAsync();

    public async Task<UserRequestData> LoginUser(UserInfo userInfo)
    {
        var hashedPassword = UserSecurity.GetHashedPassword(userInfo.Password);
        var user = await _context
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == userInfo.Login);
        if (user == null)
            throw new UserException($"User not found with login {userInfo.Login}");
        if (!UserSecurity.ArePasswordsEqual(user.PasswordHash, hashedPassword))
            throw new UserException("Wrong password");

        _logger.LogInformation("User {Login} authed", user.Login);
        var token = _jwtService.GetToken(user);
        return new(user.Id, user.Login, token);
    }

    public async Task<UserRequestData> RegisterUser(UserInfo userInfo)
    {
        var hashedPassword = UserSecurity.GetHashedPassword(userInfo.Password);
        try
        {
            var registered = _context
                .Users.Add(new User { Login = userInfo.Login, PasswordHash = hashedPassword })
                .Entity;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Registered new user with login {Login}", registered.Login);
            var token = _jwtService.GetToken(registered);
            return new(registered.Id, registered.Login, token);
        }
        catch (Exception)
        {
            throw;
            //throw new UserException($"Failed to register user with login {userInfo.Login}");
        }
    }
}
