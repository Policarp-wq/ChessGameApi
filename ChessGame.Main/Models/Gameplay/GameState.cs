using System;
using ChessGameApi.DTOs;
using ChessGameApi.Models.ChessPieces;

namespace ChessGameApi.Models.Gameplay
{
    public sealed class GameState
    {
        private readonly ChessBoard _board;
        private readonly Player _player1,
            _player2;
        private readonly Player _currentPlayer;
        public bool IsKingUnderAttack { get; private set; } = false;
        private readonly Dictionary<ChessLocation, List<ChessLocation>> _possibleMoves;
        public bool IsOver => _possibleMoves.Count == 0;

        public GameState(ChessBoard board, Player player1, Player player2, Player currentPlayer)
        {
            _board = board;
            _player1 = player1;
            _player2 = player2;
            _currentPlayer = currentPlayer;
            IsKingUnderAttack = !IsKingSafe(_board, _currentPlayer);
            _possibleMoves = FetchPlayerLegalMoves();
        }

        private GameStateDTO? _dtoCache;

        public GameStateDTO ToDTO()
        {
            if (_dtoCache == null)
            {
                _dtoCache = new GameStateDTO(_board, _currentPlayer.Id, _player1, _player2);
            }
            return _dtoCache;
        }

        private IEnumerable<BoardCell> GetEnemyCells() =>
            _board.GetCellsByPieceColor(_currentPlayer.ChessSide.GetEnemy());

        private IEnumerable<BoardCell> GetAllyCells() =>
            _board.GetCellsByPieceColor(_currentPlayer.ChessSide);

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
                var context = new MoveContext(_board, allyCell);
                foreach (var pos in allyCell.Piece!.GetPossibleMoves(context))
                {
                    var attackedCell = _board.GetCell(pos);
                    if (IsMoveAppliable(_board, _currentPlayer, allyCell, attackedCell))
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
