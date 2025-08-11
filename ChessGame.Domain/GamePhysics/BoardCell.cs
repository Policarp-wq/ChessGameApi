using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.Exceptions;

namespace ChessGame.Domain.GamePhysics
{
    public sealed class BoardCell : ICloneable
    {
        public ChessPiece? Piece { get; private set; }
        public ChessLocation Location { get; set; }

        public bool IsEmpty() => Piece == null;

        public bool IsEmptyWithAttribute() => Piece == null;

        public BoardCell(int x, int y)
        {
            Location = new ChessLocation(x, y);
        }

        public void SetPiece(ChessPiece piece)
        {
            if (Piece != null)
                throw new InvalidBoardOperationException(
                    $"Failed to set piece on the cell {Location.X} {Location.Y}: It's occupied"
                );
            Piece = piece;
        }

        public void Clear()
        {
            Piece = null;
        }

        public object Clone()
        {
            var cell = new BoardCell(Location.X, Location.Y);
            cell.Piece = Piece;
            return cell;
        }
    }
}