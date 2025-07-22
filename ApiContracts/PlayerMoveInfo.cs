using ChessGameApi.Models;

namespace ChessGameApi.ApiContracts
{
    public record PlayerMoveInfo(Guid GameId, int PlayerId, ChessLocation From, ChessLocation To)
    {
    }
}
