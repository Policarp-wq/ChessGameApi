using System.Text.Json.Serialization;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Main.DTOs
{
    public class GameStateDTO
    {
        public Guid GameId { get; set; }
        public ChessBoard Board { get; set; }
        public int CurrentPlayerId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChessColors Turn { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public GameStateDTO(
            Guid gameId,
            ChessBoard board,
            int currentPlayerId,
            ChessColors turn,
            Player player1,
            Player player2
        )
        {
            GameId = gameId;
            Board = board;
            CurrentPlayerId = currentPlayerId;
            Turn = turn;
            Player1 = player1;
            Player2 = player2;
        }

        public static GameStateDTO ToDTO(GameState state, Guid gameId)
        {
            ChessColors turn = state.CurrentPlayer.ChessSide;
            return new GameStateDTO(
                gameId,
                state.Board,
                state.CurrentPlayer.Id,
                turn,
                state.Player1,
                state.Player2
            );
        }
    }
}
