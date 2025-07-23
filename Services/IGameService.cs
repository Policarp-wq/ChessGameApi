using ChessGameApi.ApiContracts;
using ChessGameApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace ChessGameApi.Services
{
    public interface IGameService
    {
        public Guid CreateGameRequest(User requester);
        public GameState CreateGame(Guid GameId, User joiner);
        public GameState MakeMove(PlayerMoveInfo moveInfo);
        public List<ChessLocation> GetAvailableMoves(AvailableMovesRequest requests);
        public GameState GetGameState(Guid GameId);
        public bool TryJoinGame(Guid gameId, User joiner, [NotNullWhen(true)] out GameState? state);
    }
}
 