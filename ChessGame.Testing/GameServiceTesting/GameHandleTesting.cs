using ChessGame.Database.Models;
using ChessGame.Domain.Exceptions;
using ChessGame.Main.DTOs;
using ChessGame.Main.Exceptions.ResponseExceptions;
using ChessGame.Main.Services;

namespace ChessGame.Testing.GameServiceTesting
{
    public class GameHandleTesting
    {
        [Fact]
        public void GameCreatesIfDataIsNormal()
        {
            var gameService = new GameService();
            var user = new PlayerRegisterInfo(1, "test");

            var gameId = gameService.CreateGameRequest(user);
            var state = gameService.JoinByCodeAndCreateGame(gameId, new PlayerRegisterInfo(2, ""));

            Assert.NotNull(state);
        }

        [Fact]
        public void OneUserCantCreateMultipleGames()
        {
            var gameService = new GameService();
            var user = new PlayerRegisterInfo(1, "test");

            var gameId = gameService.CreateGameRequest(user);
            Assert.Throws<GameQueueException>(() => gameService.CreateGameRequest(user));
        }

        [Fact]
        public void UserCantJoinRequestedGame()
        {
            var gameService = new GameService();
            var user = new PlayerRegisterInfo(1, "test");

            var gameId = gameService.CreateGameRequest(user);
            Assert.Throws<GameServiceException>(() =>
                gameService.JoinByCodeAndCreateGame(gameId, user)
            );
        }

        // [Fact]
        // public void PlayerCanJoinStartedGame()
        // {
        //     var gameService = new GameService();
        //     var user = new PlayerRegisterInfo(1, "test");

        //     var gameId = gameService.CreateGameRequest(user);
        //     gameService.JoinByCodeAndCreateGame(gameId, new PlayerRegisterInfo(2, ""));
        //     Assert.NotNull(gameService.JoinByCodeAndCreateGame(gameId, user));
        // }

        [Fact]
        public void OtherUserCantJoinStartedGame()
        {
            var gameService = new GameService();
            var user = new PlayerRegisterInfo(1, "test");

            var gameId = gameService.CreateGameRequest(user);
            gameService.JoinByCodeAndCreateGame(gameId, new PlayerRegisterInfo(2, ""));
            Assert.False(gameService.TryRejoinGame(3, out var rejoinedGameId));
            Assert.Null(rejoinedGameId);
        }
    }
}
