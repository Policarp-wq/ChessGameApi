using ChessGame.Domain.GamePhysics;

namespace ChessGame.Main.ApiContracts
{
    public record PlayerMoveInfo(
        Guid GameId,
        int PlayerId,
        ChessLocation From,
        ChessLocation To
    ) { }
}
