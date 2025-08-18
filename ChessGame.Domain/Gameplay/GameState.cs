using System.Diagnostics.CodeAnalysis;
using ChessGame.Domain.GamePhysics;

namespace ChessGame.Domain.Gameplay
{
    public sealed class GameState
    {
        public readonly ChessBoard Board;
        public readonly Player Player1,
            Player2;
        public readonly Player CurrentPlayer;
        public bool IsKingUnderAttack { get; private set; } = false;
        private readonly Dictionary<ChessLocation, List<ChessLocation>> _possibleMoves;

        [MemberNotNullWhen(true, nameof(Result))]
        public bool IsOver => _possibleMoves.Count == 0;
        public Player NextPlayer => CurrentPlayer == Player1 ? Player2 : Player1;
        public EndgameResult? Result
        {
            get
            {
                if (!IsOver)
                    return null;
                var state = IsKingUnderAttack ? EndgameState.Checkmate : EndgameState.Stalemate;
                EndgameResult result = new(
                    state,
                    state == EndgameState.Checkmate ? NextPlayer.Id : null
                );
                return result;
            }
        }

        public GameState(ChessBoard board, Player player1, Player player2, Player currentPlayer)
        {
            Board = board;
            Player1 = player1;
            Player2 = player2;
            CurrentPlayer = currentPlayer;
            IsKingUnderAttack = !IsKingSafe(Board, CurrentPlayer);
            _possibleMoves = FetchPlayerLegalMoves();
        }

        private IEnumerable<BoardCell> GetEnemyCells() =>
            Board.GetCellsByPieceColor(CurrentPlayer.ChessSide.GetEnemy());

        private IEnumerable<BoardCell> GetAllyCells() =>
            Board.GetCellsByPieceColor(CurrentPlayer.ChessSide);

        public static IEnumerable<BoardCell> GetEnemyCells(ChessBoard board, Player current) =>
            board.GetCellsByPieceColor(current.ChessSide.GetEnemy());

        public List<ChessLocation> GetMoves(int x, int y) => GetMoves(new ChessLocation(x, y));

        public List<ChessLocation> GetMoves(ChessLocation location)
        {
            if (_possibleMoves.TryGetValue(location, out var res))
                return res;
            else
                return [];
        }

        public bool IsMovePossible(ChessLocation from, ChessLocation to) =>
            GetMoves(from).Contains(to);

        private static bool IsKingSafe(ChessBoard currentBoard, Player currentPlayer)
        {
            //fix: every time generates enumerable from start!
            foreach (var enemyCell in GetEnemyCells(currentBoard, currentPlayer))
            {
                var context = new MoveContext(currentBoard, enemyCell);
                foreach (var pos in enemyCell.Piece!.GetPossibleMoves(context))
                {
                    var attackedCell = currentBoard.GetCell(pos);
                    if (
                        attackedCell.Piece != null
                        && attackedCell.Piece.Name == ChessPieceNames.King
                    )
                        return false;
                }
            }
            return true;
        }

        private Dictionary<ChessLocation, List<ChessLocation>> FetchPlayerLegalMoves()
        {
            var possibleMoves = new Dictionary<ChessLocation, List<ChessLocation>>();
            foreach (var allyCell in GetAllyCells())
            {
                var context = new MoveContext(Board, allyCell);
                foreach (var pos in allyCell.Piece!.GetPossibleMoves(context))
                {
                    var attackedCell = Board.GetCell(pos);
                    if (IsMoveAppliable(Board, CurrentPlayer, allyCell, attackedCell))
                    {
                        if (!possibleMoves.TryAdd(allyCell.Location, [pos]))
                        {
                            possibleMoves[allyCell.Location].Add(pos);
                        }
                    }
                }
            }
            return possibleMoves;
        }

        //Utilizes MovePiece
        private static bool IsMoveAppliable(
            ChessBoard board,
            Player currentPlayer,
            BoardCell from,
            BoardCell to
        )
        {
            var before = (BoardCell)to.Clone();
            board.MovePiece(from, to);
            var isKingSafe = IsKingSafe(board, currentPlayer);
            //fix: revert move
            board.MovePiece(to, from);
            if (before.Piece == null)
                to.Clear();
            else
                to.SetPiece(before.Piece);
            return isKingSafe;
        }
    }
}
