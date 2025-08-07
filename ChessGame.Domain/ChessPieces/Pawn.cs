using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Domain.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public Pawn(ChessColors color)
            : base(color, ChessPieceNames.Pawn) { }

        public override List<ChessLocation> GetPossibleMoves(MoveContext context)
        {
            var (board, position) = context;
            var (x, y) = (position.Location.X, position.Location.Y);
            int delta = Color == ChessColors.White ? 1 : -1;
            //TODO: better solution
            bool WasMoved = !(
                Color == ChessColors.White && y == 1
                || Color == ChessColors.Black && y == ChessBoard.DIM_Y - 2
            );
            List<ChessLocation> res = [];
            if (TryMove(board, x + 1, y + delta, out BoardCell? cell) && CanAttack(cell))
                res.Add(cell!.Location);
            if (
                TryMove(board, x - 1, y + delta, out cell)
                && CanAttack(board.TryGetCell(x - 1, y + delta))
            ) //!
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
