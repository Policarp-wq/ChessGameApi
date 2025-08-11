using System;
using ChessGame.Database.Models;
using ChessGame.Main.DTOs;

namespace ChessGame.Main.Abstractions;

public interface IUserService
{
    Task<UserRequestData> LoginUser(UserInfo userInfo);
    Task<UserRequestData> RegisterUser(UserInfo userInfo);
    Task<List<User>> GetAll();
}
