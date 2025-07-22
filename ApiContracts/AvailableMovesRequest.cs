using ChessGameApi.Models;

namespace ChessGameApi.ApiContracts
{
    public record AvailableMovesRequest(Guid GameId, int PlayerId, ChessLocation From);
}
