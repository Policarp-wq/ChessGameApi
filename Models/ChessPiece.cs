namespace ChessGameApi.Models
{
    public abstract class ChessPiece
    {
        public BoardCell Cell { get; private set; }
        public readonly ChessColors Color;
        public readonly ChessPieceNames Name;

        public ChessPiece(ChessColors color, ChessPieceNames name, BoardCell cell)
        {
            Color = color;
            Name = name;
            Cell = cell;
            cell.SetPiece(this);
        }
        public virtual void Move(BoardCell cell)
        {
            Cell = cell;
        }
        public abstract List<ChessLocation> GetPossibleMoves(ChessBoard board);
        protected bool TryMove(ChessBoard board, int x, int y, out BoardCell? cell)
        {
            cell = board.GetCell(x, y);
            if (cell == null)
                return false;
            if (cell.Piece != null && cell.Piece.Color == Color)
                return false;
            return true;
        }
        protected bool CanAttack(BoardCell? cell)
        {
            if (cell == null) return false;
            return cell.Piece != null && cell.Piece.Color != Color;
        }
    }
}
