using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.GamePhysics;

namespace ChessGame.Testing.PiecesTesting
{
    public class BishopTesting
    {
        [Fact]
        public void RightMovesWhenBoardIsClean()
        {
            ChessBoard board = new();
            board.GetCell(3, 3).SetPiece(new Bishop(ChessColors.White));
            var moves = board.GetPossibleMoves(3, 3);

            List<ChessLocation> expect =
            [
                new(0, 6),
                new(1, 5),
                new(2, 4),
                new(4, 2),
                new(5, 1),
                new(6, 0),
                new(0, 0),
                new(1, 1),
                new(2, 2),
                new(4, 4),
                new(5, 5),
                new(6, 6),
                new(7, 7),
            ];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void RightMovesWhenEnemyBlocking()
        {
            ChessBoard board = new();
            board.GetCell(3, 3).SetPiece(new Bishop(ChessColors.White));
            board.GetCell(6, 6).SetPiece(new Pawn(ChessColors.Black));
            var moves = board.GetPossibleMoves(3, 3);
            List<ChessLocation> expect =
            [
                new(0, 6),
                new(1, 5),
                new(2, 4),
                new(4, 2),
                new(5, 1),
                new(6, 0),
                new(0, 0),
                new(1, 1),
                new(2, 2),
                new(4, 4),
                new(5, 5),
                new(6, 6),
            ];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void RightMovesWhenAllyBlocking()
        {
            ChessBoard board = new();
            board.GetCell(3, 3).SetPiece(new Bishop(ChessColors.White));
            board.GetCell(6, 6).SetPiece(new Pawn(ChessColors.White));
            var moves = board.GetPossibleMoves(3, 3);
            List<ChessLocation> expect =
            [
                new(0, 6),
                new(1, 5),
                new(2, 4),
                new(4, 2),
                new(5, 1),
                new(6, 0),
                new(0, 0),
                new(1, 1),
                new(2, 2),
                new(4, 4),
                new(5, 5),
            ];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }
    }
}
