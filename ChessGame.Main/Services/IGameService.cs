using System.Diagnostics.CodeAnalysis;
using ChessGame.Domain.GamePhysics;
using ChessGame.Main.ApiContracts;
using ChessGame.Main.DTOs;
using ChessGame.Main.Models;

namespace ChessGame.Main.Services
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
