using System;

namespace ChessGame.Main.Exceptions.InnerExceptions;

public class InnerExceptionBase : ApplicationException
{
    public InnerExceptionBase(string message)
        : base(message) { }
}
