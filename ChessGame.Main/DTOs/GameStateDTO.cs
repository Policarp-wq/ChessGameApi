using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Main.DTOs
{
    public record GameStateDTO(
        ChessBoard Board,
        int CurrentPlayerId,
        Player Player1,
        Player Player2
    )
    {
        public static GameStateDTO ToDTO(GameState state) =>
            new(state.Board, state.CurrentPlayer.Id, state.Player1, state.Player2);
    }
}