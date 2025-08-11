using System;
using System.Security.Cryptography;
using System.Text;

namespace ChessGame.Main.Handlers;

public static class UserSecurity
{
    public static string GetHashedPassword(string password)
    {
        Byte[] bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool ArePasswordsEqual(string expected, string hashedGiven)
    {
        return expected.Equals(hashedGiven);
    }
}
