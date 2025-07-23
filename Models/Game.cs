using ChessGameApi.Exceptions.Chess;
using ChessGameApi.Models.ChessPieces;

namespace ChessGameApi.Models
{
    public sealed class Game
    {
        private readonly ChessBoard _board;
        private readonly Player Player1;
        private readonly Player Player2;
        public readonly Guid Id;
        public Player? Winner {  get; private set; }
        public int Turn {  get; private set; }
        public Game(Player player1, Player player2, Guid id)
        {
            Id = id;
            _board = new ChessBoard();
            _board.FillBoardWithInitialState();
            if(player1.ChessSide == ChessColors.White)
            {
                Player1 = player1;
                Player2 = player2;
            }
            else
            {
                Player1 = player2;
                Player2 = player1;
            }
            Turn = 1;
        }
        public List<ChessLocation> GetPossibleMoves(ChessLocation location)
        {
            var cell = _board.TryGetCell(location);
            if (cell == null || cell.Piece == null || CurrentPlayer.ChessSide != cell.Piece.Color)
                return [];
            return _board.GetPossibleMoves(location);
        }
        public GameState MakeMove(ChessLocation from, ChessLocation to, int PlayerId)
        {
            if (CurrentPlayer.Id != PlayerId)
                throw new InvalidBoardOperationException("Attempet to make a move when other player's turn");
            var cellFrom = _board.TryGetCell(from);
            var cellTo = _board.TryGetCell(to);
            if(cellFrom == null || cellTo == null) throw new InvalidBoardOperationException("Attempted to access not existing cells");
            if (!_board.GetPossibleMoves(cellFrom.Location).Contains(cellTo.Location))
            {
                throw new InvalidBoardOperationException("Attempted to commit forbidden move");
            }
            if(cellTo.Piece != null)
            {
                CurrentPlayer.Owns.Add(cellTo.Piece);
                if(cellTo.Piece.Name == ChessPieceNames.King)
                {
                    Winner = CurrentPlayer;
                    return new GameState(_board, CurrentPlayer.Id, Player1, Player2);//Winner selection!
                }
            }
            _board.MovePiece(cellFrom, cellTo); 
            Turn++;
            return new GameState(_board, CurrentPlayer.Id, Player1, Player2);
        }
        public GameState CurrentState => new(_board, CurrentPlayer.Id, Player1, Player2); // creates multiple equal objects
        public bool IsPlayer(int userId) => Player1.Id == userId || Player2.Id == userId;
        public int GetPlayerNumber() => Turn % 2 == 0 ? 2 : 1;
        public Player CurrentPlayer => GetPlayerNumber() == 1 ? Player1 : Player2;
        public Player NextPlayer => GetPlayerNumber() == 1 ? Player2 : Player1;
    }
}
