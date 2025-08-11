using System;

namespace ChessGame.Domain.Exceptions;

public class ChessException : Exception
{
    public ChessException(string? message)
        : base(message) { }
}