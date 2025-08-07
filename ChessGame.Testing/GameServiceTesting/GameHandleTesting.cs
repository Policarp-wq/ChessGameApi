using ChessGame.Domain.Exceptions;
using ChessGame.Main.Exceptions;
using ChessGame.Main.Models;
using ChessGame.Main.Services;

namespace ChessGame.Testing.GameServiceTesting
{
    public class GameHandleTesting
    {
        [Fact]
        public void GameCreatesIfDataIsNormal()
        {
            var gameService = new GameService();
            var user = new User() { Id = 1, Name = "test" };

            var gameId = gameService.CreateGameRequest(user);
            var state = gameService.CreateGame(gameId, new User() { Id = 2, Name = "test" });

            Assert.NotNull(state);
        }

        [Fact]
        public void OneUserCantCreateMultipleGames()
        {
            var gameService = new GameService();
            var user = new User() { Id = 1, Name = "test" };

            var gameId = gameService.CreateGameRequest(user);
            Assert.Throws<GameQueueException>(() => gameService.CreateGameRequest(user));
        }

        [Fact]
        public void UserCantJoinRequestedGame()
        {
            var gameService = new GameService();
            var user = new User() { Id = 1, Name = "test" };

            var gameId = gameService.CreateGameRequest(user);
            Assert.Throws<GameServiceException>(() => gameService.JoinGame(gameId, user));
        }

        [Fact]
        public void PlayerCanJoinStartedGame()
        {
            var gameService = new GameService();
            var user = new User() { Id = 1, Name = "test" };

            var gameId = gameService.CreateGameRequest(user);
            gameService.CreateGame(gameId, new User() { Id = 2, Name = "" });
            Assert.NotNull(gameService.JoinGame(gameId, user));
        }

        [Fact]
        public void OtherUserCantJoinStartedGame()
        {
            var gameService = new GameService();
            var user = new User() { Id = 1, Name = "test" };

            var gameId = gameService.CreateGameRequest(user);
            gameService.CreateGame(gameId, new User() { Id = 2, Name = "" });
            Assert.Throws<GameServiceException>(() =>
                gameService.JoinGame(gameId, new User() { Id = 3, Name = "" })
            );
        }
    }
}
