using ChessGame.Main.Models;

namespace ChessGame.Main.ApiContracts
{
    public record GameRequest(User User, Guid GameId, DateTime Created) { }
}
