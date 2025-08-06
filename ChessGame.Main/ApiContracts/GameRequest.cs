using ChessGameApi.Models;

namespace ChessGameApi.ApiContracts
{
    public record GameRequest(User User, Guid GameId, DateTime Created) { }
}
