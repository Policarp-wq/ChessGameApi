using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGameApi.Models;
using ChessGameApi.Models.ChessPieces;
using ChessGameApi.Models.Gameplay;

namespace ChessGame.Testing.PiecesTesting
{
    public class KnightTesting
    {
        [Fact]
        public void RightMovesWhenBoardIsClean()
        {
            ChessBoard board = new();
            board.GetCell(4, 3).SetPiece(new Knight(ChessColors.White));
            var moves = board.GetPossibleMoves(4, 3);
            List<ChessLocation> expect =
            [
                new(2, 4),
                new(3, 5),
                new(5, 5),
                new(6, 4),
                new(6, 2),
                new(5, 1),
                new(3, 1),
                new(2, 2),
            ];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void RightMovesWhenBoardEnemies()
        {
            ChessBoard board = new();
            board.GetCell(4, 3).SetPiece(new Knight(ChessColors.White));
            List<ChessLocation> expect =
            [
                new(2, 4),
                new(3, 5),
                new(5, 5),
                new(6, 4),
                new(6, 2),
                new(5, 1),
                new(3, 1),
                new(2, 2),
            ];
            foreach (var p in expect)
            {
                board.GetCell(p.X, p.Y).SetPiece(new Pawn(ChessColors.Black));
            }
            var moves = board.GetPossibleMoves(4, 3);

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void NoMovesWhenBoardAllies()
        {
            ChessBoard board = new();
            board.GetCell(4, 3).SetPiece(new Knight(ChessColors.White));
            List<ChessLocation> allies =
            [
                new(2, 4),
                new(3, 5),
                new(5, 5),
                new(6, 4),
                new(6, 2),
                new(5, 1),
                new(3, 1),
                new(2, 2),
            ];
            foreach (var p in allies)
            {
                board.GetCell(p.X, p.Y).SetPiece(new Pawn(ChessColors.White));
            }
            var moves = board.GetPossibleMoves(4, 3);

            Assert.Empty(moves);
        }

        [Fact]
        public void RightMovesWhenSmallArea()
        {
            ChessBoard board = new();
            board.GetCell(0, 0).SetPiece(new Knight(ChessColors.White));
            var moves = board.GetPossibleMoves(0, 0);
            List<ChessLocation> expect = [new(1, 2), new(2, 1)];

            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }
    }
}
