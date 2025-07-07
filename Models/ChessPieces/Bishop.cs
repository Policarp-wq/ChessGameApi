
namespace ChessGameApi.Models.ChessPieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(ChessColors color, BoardCell cell) : base(color, ChessPieceNames.Bishop, cell)
        {
        }

        public override List<ChessLocation> GetPossibleMoves(ChessBoard board)
        {
            var (x, y) = (Cell.Location.X, Cell.Location.Y);
            List<ChessLocation> res = [];
            for (int i = 0; i < 2; ++i)
            {
                BoardCell? cell;
                int delta = 1 - 2 * i;
                while (TryMove(board, x + delta, y + delta, out cell) && cell!.Piece == null)
                {
                    res.Add(cell.Location);
                    delta += 1 - 2 * i;
                }
                if (CanAttack(cell))
                    res.Add(cell!.Location);
                delta = 1 - 2 * i;
                while (TryMove(board, x - delta, y + delta, out cell) && cell!.Piece == null)
                {
                    res.Add(cell.Location);
                    delta += 1 - 2 * i;
                }
                if (CanAttack(cell))
                    res.Add(cell!.Location);
            }
            return res;
        }
    }
}
