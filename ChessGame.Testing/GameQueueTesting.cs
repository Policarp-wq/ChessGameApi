using ChessGame.Database.Models;
using ChessGame.Domain.Exceptions;
using ChessGame.Main.DTOs;
using ChessGame.Main.Handlers;

namespace ChessGame.Testing
{
    public class GameQueueTesting
    {
        [Fact]
        public void UsersAddCorrectly()
        {
            var queue = new GameQueue();

            var gameId1 = queue.AddPlayer(new PlayerRegisterInfo(1, "test1"));
            var gameId2 = queue.AddPlayer(new PlayerRegisterInfo(2, "test2"));

            Assert.True(queue.TryGetUserByGameId(gameId1, out var user));
            Assert.NotNull(user);
            Assert.Equal("test1", user.Login);

            Assert.True(queue.TryGetUserByGameId(gameId2, out user));
            Assert.NotNull(user);
            Assert.Equal("test2", user.Login);
        }

        [Fact]
        public void OneUser_CantEnterQueue_MultipleTimes()
        {
            var queue = new GameQueue();
            var user = new PlayerRegisterInfo(1, "");

            queue.AddPlayer(user);
            int afterFirstEnter = queue.QueueSize;

            queue.AddPlayer(user);
            int afterSecondEnter = queue.QueueSize;

            Assert.Equal(afterFirstEnter, afterSecondEnter);
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

            queue.AddPlayer(new PlayerRegisterInfo(1, ""));
            queue.AddPlayer(new PlayerRegisterInfo(2, ""));

            queue.TryRemovePlayer(1);
            Assert.False(queue.TryGetGameId(1, out var _));
            queue.TryRemovePlayer(2);
            Assert.False(queue.TryGetGameId(2, out _));
        }
    }
}
