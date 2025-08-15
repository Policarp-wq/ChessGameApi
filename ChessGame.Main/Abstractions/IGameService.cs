using System.Diagnostics.CodeAnalysis;
using ChessGame.Database.Models;
using ChessGame.Domain.GamePhysics;
using ChessGame.Main.DTOs;
using ChessGame.Main.Models;

namespace ChessGame.Main.Abstractions
{
    public interface IGameService
    {
        void AddUserToGame(int userId, GameSession session);
        void AddUserToGame(int userId, Guid gameId);
        GameStateDTO JoinByCodeAndCreateGame(Guid gameId, PlayerRegisterInfo joiner);
        Guid CreateGameRequest(PlayerRegisterInfo requester);
        List<ChessLocation> GetAvailableMoves(AvailableMovesRequest request);
        GameStateDTO GetGameState(Guid GameId);
        GameSession GetSession(Guid gameId);
        GameStateDTO MakeMove(PlayerMoveInfo moveInfo);
        void RemoveUserFromGame(int userId, GameSession session);
        void RemoveUserFromGame(int userId);
        bool TryLeaveGame(int userId, [NotNullWhen(true)] out Guid? gameId);
        bool TryRejoinGame(int userId, [NotNullWhen(true)] out Guid? gameId);
    }
}
