using System;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Main.Models;

public class GameSession
{
    public readonly Game Game;

    public GameSession(Game game)
    {
        Game = game;
    }

    public bool IsPlayer1Connected { get; private set; } = false;
    public bool IsPlayer2Connected { get; private set; } = false;
    public bool IsAllPLayersDisconnected => !(IsPlayer1Connected || IsPlayer2Connected);
    public Guid GameId => Game.Id;

    public bool IsUserPlayer(int userId) => Game.Player1.Id == userId || Game.Player2.Id == userId;

    public void DisconnectPlayer(int id)
    {
        if (Game.Player1.Id == id)
        {
            IsPlayer1Connected = false;
        }
        if (Game.Player2.Id == id)
        {
            IsPlayer2Connected = false;
        }
    }

    public void ConnectPlayer(int id)
    {
        if (Game.Player1.Id == id)
        {
            IsPlayer1Connected = true;
        }
        if (Game.Player2.Id == id)
        {
            IsPlayer2Connected = true;
        }
    }
}
