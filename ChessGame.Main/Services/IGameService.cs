using System.Diagnostics.CodeAnalysis;
using ChessGameApi.ApiContracts;
using ChessGameApi.DTOs;
using ChessGameApi.Models;
using ChessGameApi.Models.Game;

namespace ChessGameApi.Services
{
    public interface IGameService
    {
        public Guid CreateGameRequest(User requester);
        public GameStateDTO CreateGame(Guid GameId, User joiner);
        public GameStateDTO MakeMove(PlayerMoveInfo moveInfo);
        public List<ChessLocation> GetAvailableMoves(AvailableMovesRequest requests);
        public GameStateDTO GetGameState(Guid GameId);
        public bool TryJoinGame(
            Guid gameId,
            User joiner,
            [NotNullWhen(true)] out GameStateDTO? state
        );
    }
}
