using System;
using System.Net;

namespace ChessGame.Main.Exceptions.ResponseExceptions;

public class UserException : ServerResponseException
{
    public UserException(string? message)
        : base(HttpStatusCode.Forbidden, message) { }
}
