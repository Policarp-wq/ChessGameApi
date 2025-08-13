using ChessGame.Database.Models;
using ChessGame.Domain.Exceptions;
using ChessGame.Main.Handlers;

namespace ChessGame.Testing
{
    public class GameQueueTesting
    {
        [Fact]
        public void UsersAddCorrectly()
        {
            var queue = new GameQueue();

            var gameId1 = queue.AddPlayer(new User() { Id = 1, Login = "test1" });
            var gameId2 = queue.AddPlayer(new User() { Id = 2, Login = "test2" });

            Assert.True(queue.TryGetUserByGameId(gameId1, out var user));
            Assert.NotNull(user);
            Assert.Equal("test1", user.Login);

            Assert.True(queue.TryGetUserByGameId(gameId2, out user));
            Assert.NotNull(user);
            Assert.Equal("test2", user.Login);
        }

        [Fact]
        public void OneUserCantMakeMultipleQueues()
        {
            var queue = new GameQueue();
            var user = new User() { Id = 1, Login = "GameCreatesIfDataIsNormal" };

            queue.AddPlayer(user);
            Assert.Throws<GameQueueException>(() => queue.AddPlayer(user));
        }

        [Fact]
        public void QueueReturnsFalseIfUserNotInQueue()
        {
            var queue = new GameQueue();

            Assert.False(queue.TryGetGameId(2, out var _));
        }

        [Fact]
        public void QueueReturnsFalseIfNoGameWithId()
        {
            var queue = new GameQueue();
            Assert.False(queue.TryGetUserByGameId(Guid.CreateVersion7(), out _));
        }

        [Fact]
        public void QueueRemovesUsers()
        {
            var queue = new GameQueue();

            queue.AddPlayer(new User { Id = 1, Login = "GameCreatesIfDataIsNormal" });
            queue.AddPlayer(new User { Id = 2, Login = "GameCreatesIfDataIsNormal" });

            queue.RemovePlayer(1);
            Assert.False(queue.TryGetGameId(1, out var _));
            queue.RemovePlayer(2);
            Assert.False(queue.TryGetGameId(2, out _));
        }
    }
}
