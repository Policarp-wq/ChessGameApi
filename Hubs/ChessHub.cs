using Microsoft.AspNetCore.SignalR;

namespace ChessGameApi.Hubs
{
    public interface IChessHubClient
    {
        Task UpdateBoard(string board);
    }
    public class ChessHub : Hub<IChessHubClient>
    {
        public async Task MakeMove(string player, string move)
        {
            await Clients.All.UpdateBoard(move);
        }
    }
}
