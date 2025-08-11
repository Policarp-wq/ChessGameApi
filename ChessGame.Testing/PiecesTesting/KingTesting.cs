using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.GamePhysics;

namespace ChessGame.Testing.PiecesTesting
{
    public class KingTesting
    {
        [Fact]
        public void RightMovesWhenBoardIsClean()
        {
            ChessBoard board = new();
            board.GetCell(4, 4).SetPiece(new King(ChessColors.White));
            var moves = board.GetPossibleMoves(4, 4);
            List<ChessLocation> expect =
            [
                new(3, 5),
                new(4, 5),
                new(5, 5),
                new(5, 4),
                new(5, 3),
                new(4, 3),
                new(3, 3),
                new(3, 4),
            ];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }
    }
}