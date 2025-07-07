

namespace ChessGameApi.Models.ChessPieces
{
    public class Pawn : ChessPiece
    {
        private bool IsFirst = true;

        public Pawn(ChessColors color, BoardCell cell) : base(color, ChessPieceNames.Pawn, cell)
        {
            IsFirst = true;
        }

        public override void Move(BoardCell cell)
        {
            base.Move(cell);
            if (IsFirst) 
                IsFirst = false;
        }
        public override List<ChessLocation> GetPossibleMoves(ChessBoard board)
        {
            var (x, y) = (Cell.Location.X, Cell.Location.Y);
            List<ChessLocation> res = [];
            if (TryMove(board, x + 1, y + 1, true, out ChessLocation? l))
                res.Add(l!);
            if (TryMove(board, x - 1, y + 1, true, out l))
                res.Add(l!);
            if (TryMove(board, x, y + 1, false, out l))
            {
                res.Add(l!);
                if (IsFirst && TryMove(board, x, y + 2, false, out l))
                {
                    res.Add(l!);
                }
            }
            return res;
        }

    }
}
