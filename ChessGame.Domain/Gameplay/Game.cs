using ChessGame.Domain.Exceptions;
using ChessGame.Domain.GamePhysics;

namespace ChessGame.Domain.Gameplay
{
    public sealed class Game
    {
        private readonly ChessBoard _board;
        private readonly Player Player1;
        private readonly Player Player2;
        public readonly Guid Id;
        public int Turn { get; private set; }
        public GameState CurrentState { get; private set; }
        public readonly Action<Player>? OnEndGame;

        public Game(Player player1, Player player2, Guid id)
        {
            Id = id;
            _board = new ChessBoard();
            _board.FillBoardWithInitialState();
            //decide starting player
            if (player1.ChessSide == ChessColors.White)
            {
                Player1 = player1;
                Player2 = player2;
            }
            else
            {
                Player1 = player2;
                Player2 = player1;
            }
            CurrentState = new GameState(_board, Player1, Player2, Player1);
            Turn = 1;
        }

        public List<ChessLocation> GetPossibleMoves(ChessLocation location)
        {
            var cell = _board.TryGetCell(location);
            if (cell == null || cell.Piece == null || CurrentPlayer.ChessSide != cell.Piece.Color)
                return [];
            return CurrentState.GetMoves(location);
        }

        public void EndGame(Player winner)
        {
            OnEndGame?.Invoke(winner);
        }

        public void MakeMove(ChessLocation from, ChessLocation to, int PlayerId)
        {
            if (CurrentPlayer.Id != PlayerId)
                throw new InvalidBoardOperationException(
                    "Attempted to make a move when other player's turn"
                );
            var cellFrom = _board.TryGetCell(from);
            var cellTo = _board.TryGetCell(to);
            if (cellFrom == null || cellTo == null)
                throw new InvalidBoardOperationException("Attempted to access not existing cells");
            if (!CurrentState.IsMovePossible(from, to))
                throw new InvalidBoardOperationException("Attempted to commit forbidden move");

            var result = _board.MovePiece(cellFrom, cellTo);
            if (result != null)
                CurrentPlayer.Owns.Add(result);
            var nextState = new GameState(_board, Player1, Player2, NextPlayer);
            if (nextState.IsOver)
            {
                EndGame(CurrentPlayer);
            }
            CurrentState = nextState;
            Turn++;
        }

        public bool IsPlayer(int userId) => Player1.Id == userId || Player2.Id == userId;

        public int GetPlayerNumber() => Turn % 2 == 0 ? 2 : 1;

        public Player CurrentPlayer => GetPlayerNumber() == 1 ? Player1 : Player2;
        public Player NextPlayer => GetPlayerNumber() == 1 ? Player2 : Player1;
    }
}