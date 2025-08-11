using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Domain.ChessPieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(ChessColors color)
            : base(color, ChessPieceNames.Bishop) { }

        public override List<ChessLocation> GetPossibleMoves(MoveContext context)
        {
            var (board, position) = context;
            var (x, y) = (position.Location.X, position.Location.Y);
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