using System.Text.Json.Serialization;

namespace ChessGame.Domain.Gameplay;

public class EndgameResult
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EndgameState State { get; set; }
    public int? WinnerId;

    public EndgameResult(EndgameState state, int? winnerId)
    {
        State = state;
        WinnerId = winnerId;
    }
}
