using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChessGame.Database.Models;
using ChessGame.Main.Abstractions;
using ChessGame.Main.Config;
using JWT.Builder;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChessGame.Main.Services;

public class JwtService : IJwtService
{
    public const string UserIdentifier = "UserId";
    public const string UserLogin = "Login";
    private readonly JwtConfig _jwtConfig;

    public JwtService(IOptions<JwtConfig> options)
    {
        _jwtConfig = options.Value;
    }

    public string GetToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            _jwtConfig.GetSecurityKey(),
            SecurityAlgorithms.HmacSha256
        );
        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: new[]
            {
                new Claim(UserIdentifier, user.Id.ToString()),
                new Claim(UserLogin, user.Login),
            },
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
