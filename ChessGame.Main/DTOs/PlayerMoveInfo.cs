using ChessGame.Domain.GamePhysics;

namespace ChessGame.Main.DTOs
{
    public record PlayerMoveInfo(
        Guid GameId,
        int PlayerId,
        ChessLocation From,
        ChessLocation To
    ) { }
}
