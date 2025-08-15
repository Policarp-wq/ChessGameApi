using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ChessGame.Database.Models;
using ChessGame.Domain.Exceptions;
using ChessGame.Main.DTOs;

namespace ChessGame.Main.Handlers
{
    public sealed class GameQueue
    {
        private readonly ConcurrentDictionary<Guid, GameRequest> _pendingRequests;
        private readonly ConcurrentDictionary<int, GameRequest> _waitingPlayers;
        private static readonly TimeSpan REQUEST_LIFETIME = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan COLLECTING_FREQ = TimeSpan.FromMinutes(1);

        public GameQueue()
        {
            _pendingRequests = new ConcurrentDictionary<Guid, GameRequest>();
            _waitingPlayers = new ConcurrentDictionary<int, GameRequest>();
        }

        public Guid AddPlayer(PlayerRegisterInfo playerInfo)
        {
            var gameRequest = new GameRequest(playerInfo, Guid.CreateVersion7(), DateTime.UtcNow);
            if (!_waitingPlayers.TryAdd(playerInfo.Id, gameRequest))
                throw new GameQueueException("User with this id is already waiting for game");

            while (!_pendingRequests.TryAdd(gameRequest.GameId, gameRequest))
                gameRequest = new GameRequest(playerInfo, Guid.CreateVersion7(), DateTime.UtcNow);
            _waitingPlayers[playerInfo.Id] = gameRequest;
            return gameRequest.GameId;
        }

        public bool TryGetUserByGameId(
            Guid gameId,
            [NotNullWhen(true)] out PlayerRegisterInfo? user
        )
        {
            user = null;
            if (_pendingRequests.TryGetValue(gameId, out var request))
            {
                user = request.Player;
                return true;
            }
            return false;
        }

        public bool TryGetGameId(int userId, [NotNullWhen(true)] out Guid? gameId)
        {
            gameId = null;
            if (_waitingPlayers.TryGetValue(userId, out var request))
            {
                gameId = request.GameId;
                return true;
            }
            return false;
        }

        public void TryRemovePlayer(int userId)
        {
            if (_waitingPlayers.TryRemove(userId, out var gameRequest))
                _pendingRequests.TryRemove(gameRequest.GameId, out var _);
        }

        // public bool TryRemove(Guid gameId)
        // {
        //     if (_pendingRequests.TryRemove(gameId, out var info))
        //     {
        //         _waitingPlayers.TryRemove(info.Player.Id, out _);
        //         return true;
        //     }
        //     return false;
        // }

        public async Task StartExpiredRequestCollector(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(COLLECTING_FREQ, token);
                ClearRequests();
            }
        }

        public void ClearRequests()
        {
            var now = DateTime.UtcNow;
            foreach (var (id, request) in _pendingRequests.ToList())
            {
                if ((now - request.Created) >= REQUEST_LIFETIME)
                {
                    _pendingRequests.TryRemove(id, out _);
                }
            }
        }
    }
}
