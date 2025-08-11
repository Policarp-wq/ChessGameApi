using System;

namespace ChessGame.Main.Exceptions.InnerExceptions;

public class EnvMissingException : InnerExceptionBase
{
    public EnvMissingException(string envName)
        : base($"Missing environment variable: {envName}") { }
}
