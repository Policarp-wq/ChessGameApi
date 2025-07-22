

namespace ChessGameApi.Models.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public bool WasMoved;

        public Pawn(ChessColors color) : base(color, ChessPieceNames.Pawn)
        {
            WasMoved = false;
        }
        public override void OnMoved(BoardCell target)
        {
            base.OnMoved(target);
            if (!WasMoved)
                WasMoved = true;
        }
        public override List<ChessLocation> GetPossibleMoves(ChessBoard board, BoardCell position)
        {
            var (x, y) = (position.Location.X, position.Location.Y);
            int delta = Color == ChessColors.White ? 1 : -1;
            List<ChessLocation> res = [];
            if (TryMove(board, x + 1, y + delta, out BoardCell? cell) && CanAttack(cell))
                res.Add(cell!.Location);
            if (TryMove(board, x - 1, y + delta, out cell) && CanAttack(board.TryGetCell(x - 1, y + delta))) //!
                res.Add(cell!.Location);
            if (TryMove(board, x, y + delta, out cell) && cell!.Piece == null)
            {
                res.Add(cell!.Location);
                if (!WasMoved && TryMove(board, x, y + 2 * delta, out cell) && cell!.Piece == null)
                {
                    res.Add(cell!.Location);
                }
            }
            return res;
        }
        

    }
}
