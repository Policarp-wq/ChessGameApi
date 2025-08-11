using System;
using ChessGame.Database.Models;

namespace ChessGame.Main.Abstractions;

public interface IJwtService
{
    public string GetToken(User user);
}
