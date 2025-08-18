using System;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
using ChessGame.Main.Exceptions.ResponseExceptions;

namespace ChessGame.Main.Models;

public class GameSession
{
    private readonly Game _game;

    public GameSession(Game game)
    {
        _game = game;
    }

    public bool IsPaused => !(IsPlayer1Connected && IsPlayer2Connected);

    public bool IsPlayer1Connected { get; private set; } = false;
    public bool IsPlayer2Connected { get; private set; } = false;
    public bool IsAllPlayersDisconnected => !(IsPlayer1Connected || IsPlayer2Connected);
    public Guid GameId => _game.Id;
    public int Player1Id => _game.Player1.Id;
    public int Player2Id => _game.Player2.Id;

    public bool IsUserPlayer(int userId) =>
        _game.Player1.Id == userId || _game.Player2.Id == userId;

    public void DisconnectPlayer(int id)
    {
        if (_game.Player1.Id == id)
        {
            IsPlayer1Connected = false;
        }
        if (_game.Player2.Id == id)
        {
            IsPlayer2Connected = false;
        }
    }

    public void ConnectPlayer(int id)
    {
        if (_game.Player1.Id == id)
        {
            IsPlayer1Connected = true;
        }
        if (_game.Player2.Id == id)
        {
            IsPlayer2Connected = true;
        }
    }

    public GameState CurrentState => _game.CurrentState;

    public List<ChessLocation> GetPossibleMoves(int playerId, ChessLocation from)
    {
        if (_game.CurrentPlayer.Id != playerId)
            return [];
        if (IsPaused)
            throw new GameSessionException($"Game {GameId} is paused");
        return _game.GetPossibleMoves(from);
    }

    private void EnsurePlayerCanMakeMove(int playerId)
    {
        if (!IsUserPlayer(playerId))
            throw new GameSessionException($"User {playerId} is not playing in this game");
        if (IsPaused)
            throw new GameSessionException($"Game {GameId} is paused");
    }

    public void MakeMove(int playerId, ChessLocation from, ChessLocation to)
    {
        EnsurePlayerCanMakeMove(playerId);
        _game.MakeMove(from, to, playerId);
    }
}
