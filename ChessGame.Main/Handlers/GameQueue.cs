using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ChessGameApi.ApiContracts;
using ChessGameApi.Exceptions.Chess;
using ChessGameApi.Models;

namespace ChessGameApi.Handlers
{
    public sealed class GameQueue
    {
        private readonly ConcurrentDictionary<Guid, GameRequest> _pendingRequests;
        private readonly ConcurrentDictionary<int, GameRequest> _waitingUsers;
        private static readonly TimeSpan REQUEST_LIFETIME = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan COLLECTING_FREQ = TimeSpan.FromMinutes(1);

        public GameQueue()
        {
            _pendingRequests = new ConcurrentDictionary<Guid, GameRequest>();
            _waitingUsers = new ConcurrentDictionary<int, GameRequest>();
        }

        public Guid AddUser(User user)
        {
            var gameRequest = new GameRequest(user, Guid.CreateVersion7(), DateTime.UtcNow);
            if (!_waitingUsers.TryAdd(user.Id, gameRequest))
                throw new GameQueueException("User with this id is already waiting for game");

            while (!_pendingRequests.TryAdd(gameRequest.GameId, gameRequest))
                gameRequest = new GameRequest(user, Guid.CreateVersion7(), DateTime.UtcNow);
            _waitingUsers[user.Id] = gameRequest;
            return gameRequest.GameId;
        }

        public bool TryGetUserByGameId(Guid gameId, [NotNullWhen(true)] out User? user)
        {
            user = null;
            if (_pendingRequests.TryGetValue(gameId, out var request))
            {
                user = request.User;
                return true;
            }
            return false;
        }

        public bool TryGetGameId(int userId, [NotNullWhen(true)] out Guid? gameId)
        {
            gameId = null;
            if (_waitingUsers.TryGetValue(userId, out var request))
            {
                gameId = request.GameId;
                return true;
            }
            return false;
        }

        public void RemoveUser(int userId)
        {
            if (_waitingUsers.TryRemove(userId, out var gameRequest))
                _pendingRequests.TryRemove(gameRequest.GameId, out var _);
        }

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
