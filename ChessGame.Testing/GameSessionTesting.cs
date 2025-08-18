using System;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
using ChessGame.Main.Exceptions.ResponseExceptions;
using ChessGame.Main.Models;

namespace ChessGame.Testing;

public class GameSessionTesting
{
    private static (Player player1, Player player2) CreatePlayers() =>
        (new Player(1, "", ChessColors.White, []), new Player(2, "", ChessColors.White, []));

    [Fact]
    public void GameId_InsideSession_MatchesProvidedGame()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);

        Assert.Equal(game.Id, session.GameId);
    }

    [Fact]
    public void PlayerIds_InsideSession_MatchesProvidedGame()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);

        Assert.Equal(game.Player1.Id, session.Player1Id);
        Assert.Equal(game.Player2.Id, session.Player2Id);
    }

    [Fact]
    public void PlayersAreDisconnected_ByDefault()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);

        Assert.False(session.IsPlayer1Connected);
        Assert.False(session.IsPlayer2Connected);
        Assert.True(session.IsAllPlayersDisconnected);
    }

    [Fact]
    public void PlayersAreConnected_ById()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);
        session.ConnectPlayer(1);
        Assert.True(session.IsPlayer1Connected);

        session.ConnectPlayer(2);
        Assert.True(session.IsPlayer2Connected);

        Assert.False(session.IsAllPlayersDisconnected);
    }

    [Fact]
    public void IfIdIsNotPresent_PLayerIsNotConnected()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);
        session.ConnectPlayer(100);

        Assert.False(session.IsPlayer1Connected);
        Assert.False(session.IsPlayer2Connected);
        Assert.True(session.IsAllPlayersDisconnected);
    }

    [Fact]
    public void GameIsPaused_WhenPlayer_Disconnects()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);
        session.ConnectPlayer(1);
        session.ConnectPlayer(2);

        Assert.False(session.IsPaused);
        session.DisconnectPlayer(1);

        Assert.True(session.IsPaused);
    }

    [Fact]
    public void ThrowsException_WhenAttemptsTo_MoveWhileGame_IsPaused()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);
        Assert.True(session.IsPaused);

        Assert.Throws<GameSessionException>(() => session.MakeMove(1, new(1, 1), new(1, 3)));
    }

    [Fact]
    public void ThrowsException_WhenAttemptsTo_MoveByPlayer_WhoIsNotItSession()
    {
        var (player1, player2) = CreatePlayers();
        var game = new Game(player1, player2, Guid.CreateVersion7());

        var session = new GameSession(game);
        session.ConnectPlayer(1);
        session.ConnectPlayer(2);
        Assert.False(session.IsPaused);

        Assert.Throws<GameSessionException>(() => session.MakeMove(100, new(1, 1), new(1, 3)));
    }
}
