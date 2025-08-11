using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.GamePhysics;

namespace ChessGame.Testing.PiecesTesting
{
    public class PawnTesting
    {
        [Fact]
        public void RightMovesWhenBoardIsClean()
        {
            ChessBoard board = new();
            board.GetCell(1, 1).SetPiece(new Pawn(ChessColors.White));
            var moves = board.GetPossibleMoves(1, 1);
            List<ChessLocation> expect = [new ChessLocation(1, 2), new ChessLocation(1, 3)];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(moves.Contains));
        }

        [Fact]
        public void OneMoveWhenEnemyOneCellAway()
        {
            ChessBoard board = new();
            board.GetCell(1, 1).SetPiece(new Pawn(ChessColors.White));
            board.GetCell(1, 3).SetPiece(new Pawn(ChessColors.Black));
            var moves = board.GetPossibleMoves(1, 1);
            List<ChessLocation> expect = [new ChessLocation(1, 2)];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(moves.Contains));
        }

        [Fact]
        public void CanMoveOnEnemies()
        {
            ChessBoard board = new();
            board.GetCell(1, 1).SetPiece(new Pawn(ChessColors.White));
            board.GetCell(2, 2).SetPiece(new Pawn(ChessColors.Black));
            board.GetCell(0, 2).SetPiece(new Pawn(ChessColors.Black));
            var moves = board.GetPossibleMoves(1, 1);
            List<ChessLocation> expect =
            [
                new ChessLocation(1, 2),
                new ChessLocation(1, 3),
                new ChessLocation(2, 2),
                new ChessLocation(0, 2),
            ];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(moves.Contains));
        }

        [Fact]
        public void CanMoveOnlyOneCellAfterFirstMoveDone()
        {
            ChessBoard board = new();
            board.TryGetCell(1, 1)!.SetPiece(new Pawn(ChessColors.White));
            board.MovePiece(board.GetCell(1, 1), board.GetCell(1, 2));

            var moves = board.GetPossibleMoves(1, 2);
            List<ChessLocation> expect = [new ChessLocation(1, 3)];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void NoMovesWhenOtherIsClose()
        {
            ChessBoard board = new();
            board.GetCell(1, 1).SetPiece(new Pawn(ChessColors.White));
            board.GetCell(1, 2).SetPiece(new Pawn(ChessColors.Black));
            var moves = board.GetPossibleMoves(1, 1);

            Assert.Empty(moves);
        }
    }
}
