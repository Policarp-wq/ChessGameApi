using ChessGame.Domain.GamePhysics;

namespace ChessGame.Main.DTOs
{
    public record AvailableMovesRequest(Guid GameId, int PlayerId, ChessLocation From);
}