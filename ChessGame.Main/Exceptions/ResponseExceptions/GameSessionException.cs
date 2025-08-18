using System;
using System.Net;

namespace ChessGame.Main.Exceptions.ResponseExceptions;

public class GameSessionException : ServerResponseException
{
    public GameSessionException(string? message)
        : base(StatusCodes.Status400BadRequest, message) { }
}
