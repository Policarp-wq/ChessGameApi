using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ChessGame.Main.Config;

public class JwtConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;

    public SymmetricSecurityKey GetSecurityKey() => new(Encoding.UTF8.GetBytes(SecretKey));
}
