using ChessGame.Database.Models;

namespace ChessGame.Main.DTOs
{
    public record GameRequest(PlayerRegisterInfo Player, Guid GameId, DateTime Created) { }
}
