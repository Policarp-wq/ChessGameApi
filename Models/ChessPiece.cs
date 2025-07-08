namespace ChessGameApi.Models
{
    public abstract class ChessPiece
    {
        public readonly ChessColors Color;
        public readonly ChessPieceNames Name;

        public ChessPiece(ChessColors color, ChessPieceNames name)
        {
            Color = color;
            Name = name;
        }
        public virtual void OnMoved(BoardCell target)
        {

        }
        public abstract List<ChessLocation> GetPossibleMoves(ChessBoard board, BoardCell position);
        protected bool TryMove(ChessBoard board, int x, int y, out BoardCell? cell)
        {
            cell = board.TryGetCell(x, y);
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
