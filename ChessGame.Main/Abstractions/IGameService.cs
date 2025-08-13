using System.Diagnostics.CodeAnalysis;
using ChessGame.Database.Models;
using ChessGame.Domain.GamePhysics;
using ChessGame.Main.DTOs;

namespace ChessGame.Main.Abstractions
{
    public interface IGameService
    {
        public Guid CreateGameRequest(PlayerRegisterInfo requester);
        public GameStateDTO CreateGame(Guid GameId, PlayerRegisterInfo joiner);
        public GameStateDTO MakeMove(PlayerMoveInfo moveInfo);
        public List<ChessLocation> GetAvailableMoves(AvailableMovesRequest requests);
        public GameStateDTO GetGameState(Guid GameId);
        public bool TryJoinGame(
            Guid gameId,
            PlayerRegisterInfo joiner,
            [NotNullWhen(true)] out GameStateDTO? state
        );
    }
}
