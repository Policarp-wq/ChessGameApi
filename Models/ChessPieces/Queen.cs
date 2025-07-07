
namespace ChessGameApi.Models.ChessPieces
{
    public class Queen : ChessPiece
    {
        public Queen(ChessColors color, BoardCell cell) : base(color, ChessPieceNames.Queen, cell)
        {
        }
        //Just a comb Bishop + Rook
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
            for (int i = 0; i < 2; ++i)
            {
                BoardCell? cell;
                int delta = 1 - 2 * i;
                while (TryMove(board, x, y + delta, out cell) && cell!.Piece == null)
                {
                    res.Add(cell.Location);
                    delta += 1 - 2 * i;
                }
                if (CanAttack(cell))
                    res.Add(cell!.Location);
                delta = 1 - 2 * i;
                while (TryMove(board, x + delta, y, out cell) && cell!.Piece == null)
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
