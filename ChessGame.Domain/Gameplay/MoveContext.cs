using ChessGame.Domain.GamePhysics;

namespace ChessGame.Domain.Gameplay
{
    public record MoveContext(ChessBoard Board, BoardCell Position);
}