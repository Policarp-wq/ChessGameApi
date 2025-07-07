using ChessGameApi.Exceptions.Chess;

namespace ChessGameApi.Models
{
    public sealed class BoardCell
    {
        public ChessPiece? Piece { get; private set; }
        public ChessLocation Location { get; set; }

        public BoardCell(int x, int y)
        {
            Location = new ChessLocation(x, y);
        }
        public void SetPiece(ChessPiece piece)
        {
            if (Piece != null)
                throw new InvalidBoardOperationException($"Failed to set piece on the cell {Location.X} {Location.Y}: It's occupied");
            Piece = piece;
            piece.Move(this);
        }
        public void Clear()
        {
            Piece = null;
        }
    }
}