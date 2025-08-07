using ChessGame.Domain.GamePhysics;

namespace ChessGame.Main.ApiContracts
{
    public record AvailableMovesRequest(Guid GameId, int PlayerId, ChessLocation From);
}
