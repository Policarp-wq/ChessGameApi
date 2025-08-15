using System;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
using ChessGame.Main.DTOs;

namespace ChessGame.Main.Handlers;

public static class SideDecider
{
    public static (Player, Player) CreatePlayers(
        PlayerRegisterInfo firstPlayer,
        PlayerRegisterInfo secondPlayer
    )
    {
        int number = DecideStartingPlayerNumber();
        Player player1,
            player2;
        if (number == 1)
        {
            player1 = new Player(firstPlayer.Id, firstPlayer.Login, ChessColors.White, []);
            player2 = new Player(secondPlayer.Id, secondPlayer.Login, ChessColors.Black, []);
        }
        else
        {
            player1 = new Player(firstPlayer.Id, firstPlayer.Login, ChessColors.Black, []);
            player2 = new Player(secondPlayer.Id, secondPlayer.Login, ChessColors.White, []);
        }
        return (player1, player2);
    }

    private static int DecideStartingPlayerNumber()
    {
        return Random.Shared.Next(1, 3);
    }
}
