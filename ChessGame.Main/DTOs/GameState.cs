using ChessGameApi.Models;
using ChessGameApi.Models.Gameplay;

namespace ChessGameApi.DTOs
{
    public record GameStateDTO(
        ChessBoard Board,
        int CurrentPlayerId,
        Player Player1,
        Player Player2
    ) { }
}
