using ChessGameApi.Models.Gameplay;

namespace ChessGameApi.Models.ChessPieces
{
    public class King : ChessPiece
    {
        public King(ChessColors color)
            : base(color, ChessPieceNames.King) { }

        //King can go on checkmate
        public override List<ChessLocation> GetPossibleMoves(MoveContext context)
        {
            var (board, position) = context;
            var (x, y) = (position.Location.X, position.Location.Y);
            List<ChessLocation> res = [];
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (TryMove(board, x + j, y + i, out BoardCell? cell))
                        res.Add(cell!.Location);
                }
            }
            return res;
        }
    }
}
