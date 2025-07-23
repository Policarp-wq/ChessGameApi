namespace ChessGameApi.Models
{
    public record GameState(ChessBoard Board, int CurrentPlayerId, Player Player1, Player Player2)
    {
    }
}
