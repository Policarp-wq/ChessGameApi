
namespace ChessGameApi.Models.ChessPieces
{
    public class King : ChessPiece
    {
        public King(ChessColors color, BoardCell cell) : base(color, ChessPieceNames.King, cell)
        {
        }
        //King can go on checkmate
        public override List<ChessLocation> GetPossibleMoves(ChessBoard board)
        {
            var (x, y) = (Cell.Location.X, Cell.Location.Y);
            List<ChessLocation> res = [];
            for(int i = -1; i < 2; ++i)
            {
                for(int j = -1; j < 2; ++j)
                {
                    if(i == 0 && j == 0)
                        continue;
                    if (TryMove(board, x + j, y + i, out BoardCell? cell))
                        res.Add(cell!.Location);
                }
            }
            return res;
        }
    }
}
