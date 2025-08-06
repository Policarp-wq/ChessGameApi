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
    public class RookTesting
    {
        [Fact]
        public void RightMovesWhenBoardIsClean()
        {
            ChessBoard board = new();

            var cell = board.GetCell(3, 3);
            cell.SetPiece(new Rook(ChessColors.White));

            var moves = board.GetPossibleMoves(3, 3);
            List<ChessLocation> expect = [];
            for (int i = 0; i < 8; ++i)
            {
                if (i != cell.Location.Y)
                    expect.Add(new(cell.Location.X, i));
            }
            for (int i = 0; i < 8; ++i)
            {
                if (i != cell.Location.X)
                    expect.Add(new(i, cell.Location.X));
            }
            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void RightMovesWhenEnemyBlocking()
        {
            ChessBoard board = new();
            var cell = board.GetCell(3, 3);
            var enemyCell = board.GetCell(3, 5);
            cell.SetPiece(new Rook(ChessColors.White));
            enemyCell.SetPiece(new Pawn(ChessColors.Black));

            var moves = board.GetPossibleMoves(3, 3);

            List<ChessLocation> expect = [];
            for (int i = 0; i < 8; ++i)
            {
                if (i == enemyCell.Location.Y + 1)
                    break;
                if (i != cell.Location.Y)
                    expect.Add(new(cell.Location.X, i));
            }
            for (int i = 0; i < 8; ++i)
            {
                if (i != cell.Location.X)
                    expect.Add(new(i, cell.Location.X));
            }
            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }

        [Fact]
        public void RightMovesWhenAllyBlocking()
        {
            ChessBoard board = new();
            var cell = board.GetCell(3, 3);
            var allyCell = board.GetCell(3, 5);
            cell.SetPiece(new Rook(ChessColors.White));
            allyCell.SetPiece(new Pawn(ChessColors.White));

            var moves = board.GetPossibleMoves(3, 3);
            List<ChessLocation> expect = [];
            for (int i = 0; i < 8; ++i)
            {
                if (i == allyCell.Location.Y)
                    break;
                if (i != cell.Location.Y)
                    expect.Add(new(cell.Location.X, i));
            }
            for (int i = 0; i < 8; ++i)
            {
                if (i != cell.Location.X)
                    expect.Add(new(i, cell.Location.X));
            }
            Assert.Equal(expect.Count, moves.Count);
            Assert.True(expect.All(e => moves.Contains(e)));
        }
    }
}
