using System;
using ChessGame.Main.Exceptions.InnerExceptions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChessGame.Main.Handlers;

public static class EnvProvider
{
    public static readonly string DB_LINK = "ASP_DB_LINK";
    public static readonly string JWT_SECRET = "ASP_JWT_SECRET";

    public static string GetEnvVal(string env)
    {
        var val = Environment.GetEnvironmentVariable(env) ?? throw new EnvMissingException(env);
        return val;
    }
}
