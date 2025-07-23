using ChessGameApi.Models.Gameplay;

namespace ChessGameApi.Models.Game
{
    public record GameState(ChessBoard Board, int CurrentPlayerId, Player Player1, Player Player2)
    {
    }
}
