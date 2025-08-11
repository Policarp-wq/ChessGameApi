using ChessGame.Database.Models;

namespace ChessGame.Main.DTOs
{
    public record GameRequest(User User, Guid GameId, DateTime Created) { }
}