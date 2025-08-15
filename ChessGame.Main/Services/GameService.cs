using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ChessGame.Database.Models;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
using ChessGame.Main.Abstractions;
using ChessGame.Main.DTOs;
using ChessGame.Main.Exceptions;
using ChessGame.Main.Exceptions.ResponseExceptions;
using ChessGame.Main.Handlers;
using ChessGame.Main.Models;

namespace ChessGame.Main.Services
{
    public sealed class GameService : IGameService
    {
        //Игры не удаляются
        private readonly ConcurrentDictionary<Guid, GameSession> _sessions = new();
        private readonly ConcurrentDictionary<int, Guid> _userToGameId = new();
        private readonly GameQueue _queue = new();

        public GameService() { }

        public void AddUserToGame(int userId, GameSession session)
        {
            _queue.TryRemovePlayer(userId);
            session.ConnectPlayer(userId);
            _userToGameId[userId] = session.GameId;
        }

        public void AddUserToGame(int userId, Guid gameId)
        {
            if (!_sessions.TryGetValue(gameId, out var session))
                throw new GameServiceException($"No game with id {gameId}");
            AddUserToGame(userId, session);
        }

        public void RemoveUserFromGame(int userId, GameSession session)
        {
            session.DisconnectPlayer(userId);
            _userToGameId.TryRemove(userId, out _);
        }

        public void RemoveUserFromGame(int userId)
        {
            if (TryGetSession(userId, out var session))
                RemoveUserFromGame(userId, session);
        }

        public GameStateDTO JoinByCodeAndCreateGame(Guid gameId, PlayerRegisterInfo joiner)
        {
            if (_sessions.TryGetValue(gameId, out _))
                throw new GameServiceException($"Game with id {gameId} is already exist");
            if (!_queue.TryGetUserByGameId(gameId, out var requester))
                throw new GameServiceException($"No game request with this id {gameId}");
            if (joiner.Id == requester.Id)
                throw new GameServiceException($"Same user cant create the game");

            var (player1, player2) = SideDecider.CreatePlayers(requester, joiner);
            var session = new GameSession(new Game(player1, player2, gameId));
            _sessions[gameId] = session;
            AddUserToGame(player1.Id, session);
            AddUserToGame(player2.Id, session);
            return GameStateDTO.ToDTO(session.Game.CurrentState, gameId);
        }

        public Guid CreateGameRequest(PlayerRegisterInfo requester) => _queue.AddPlayer(requester);

        public List<ChessLocation> GetAvailableMoves(AvailableMovesRequest request)
        {
            if (!_sessions.TryGetValue(request.GameId, out var session))
                throw new GameServiceException($"No game with id {request.GameId}");
            if (!session.IsUserPlayer(request.PlayerId))
                throw new GameServiceException(
                    $"User {request.PlayerId} is not playing in this game"
                );
            if (session.Game.CurrentPlayer.Id != request.PlayerId)
                return [];
            return session.Game.GetPossibleMoves(request.From);
        }

        public GameStateDTO GetGameState(Guid GameId)
        {
            if (!_sessions.TryGetValue(GameId, out var session))
                throw new InvalidOperationException($"No game with id {GameId}");
            return GameStateDTO.ToDTO(session.Game.CurrentState, GameId);
        }

        public GameStateDTO MakeMove(PlayerMoveInfo moveInfo)
        {
            if (!_sessions.TryGetValue(moveInfo.GameId, out var session))
                throw new InvalidOperationException($"No game with id {moveInfo.GameId}");
            if (!session.IsUserPlayer(moveInfo.PlayerId))
                throw new GameServiceException(
                    $"User {moveInfo.PlayerId} is not playing in this game"
                );
            session.Game.MakeMove(moveInfo.From, moveInfo.To, moveInfo.PlayerId);
            return GameStateDTO.ToDTO(session.Game.CurrentState, moveInfo.GameId);
        }

        public GameSession GetSession(Guid gameId)
        {
            if (!_sessions.TryGetValue(gameId, out var session))
                throw new GameServiceException($"No session with id {gameId}");
            return session;
        }

        private bool TryGetSession(int userId, [NotNullWhen(true)] out GameSession? session)
        {
            session = null;
            return _userToGameId.TryGetValue(userId, out var gameId)
                && _sessions.TryGetValue(gameId, out session)
                && session.IsUserPlayer(userId);
        }

        public bool TryRejoinGame(int userId, [NotNullWhen(true)] out Guid? gameId)
        {
            gameId = null;
            if (TryGetSession(userId, out var session))
            {
                session.ConnectPlayer(userId);
                gameId = session.GameId;
                return true;
            }
            return false;
        }

        public bool TryLeaveGame(int userId, [NotNullWhen(true)] out Guid? gameId)
        {
            gameId = null;
            if (TryGetSession(userId, out var session))
            {
                session.DisconnectPlayer(userId);
                if (session.IsAllPLayersDisconnected)
                {
                    OnAllPlayersDisconnected(session);
                    return false;
                }
                gameId = session.GameId;
                return true;
            }
            return false;
        }

        private void RemoveSession(Guid gameId)
        {
            if (_sessions.TryGetValue(gameId, out var session))
            {
                RemoveUserFromGame(session.Game.Player1.Id);
                RemoveUserFromGame(session.Game.Player2.Id);
                _sessions.TryRemove(gameId, out _);
            }
        }

        private void OnAllPlayersDisconnected(GameSession session)
        {
            RemoveSession(session.GameId);
        }
    }
}
