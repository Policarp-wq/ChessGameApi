
namespace ChessGameApi.Models.ChessPieces
{
    public class Knight : ChessPiece
    {
        public Knight(ChessColors color, BoardCell cell) : base(color, ChessPieceNames.Knight, cell)
        {
        }

        public override List<ChessLocation> GetPossibleMoves(ChessBoard board)
        {
            var (x, y) = (Cell.Location.X, Cell.Location.Y);
            List<ChessLocation> res = [];
            int[,] moves = {
                { -2,  1 }, { -1,  2 }, { 1,  2 }, { 2,  1 },
                {  2, -1 }, {  1, -2 }, { -1, -2 }, { -2, -1 }
            };

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                if (TryMove(board, x + moves[i, 0], y + moves[i, 1], out BoardCell? cell))
                    res.Add(cell!.Location);
            }
            return res;
        }
    }
}
