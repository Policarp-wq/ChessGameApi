namespace ChessGameApi.Models
{
    public record GameState(ChessBoard Board, int CurrentPlayerId, bool IsOver, Player Player1, Player Player2)
    {
    }
}
