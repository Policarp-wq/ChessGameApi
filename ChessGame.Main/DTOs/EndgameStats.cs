using System;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Main.DTOs;

public record EndgameStats
{
    public EndgameStats(int? winnerId, EndgameState result)
    {
        WinnerId = winnerId;
        Result = result;
    }

    public int? WinnerId { get; set; }

    public EndgameState Result { get; set; }
}
