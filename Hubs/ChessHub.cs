using ChessGameApi.ApiContracts;
using ChessGameApi.Models;
using ChessGameApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChessGameApi.Hubs
{
    public interface IChessHubClient
    {
        Task UpdateBoard(GameState state);
        Task JoinedGame(GameState state);
        //Task GameCreated(GameState state);
        Task ReceiveGameInviteCode(Guid gameCode);
        Task ReceiveMoves(List<ChessLocation> locations);
    }
    public class ChessHub : Hub<IChessHubClient>
    {
        private readonly IGameService _gameService;

        public ChessHub(IGameService gameService)
        {
            _gameService = gameService;
        }
        public async Task CreateGame(User user)
        {
            var gameId = _gameService.CreateGameRequest(user);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Caller.ReceiveGameInviteCode(gameId);
        }
        public async Task JoinGame(Guid gameId, User joiner)
        {
            if(!_gameService.TryJoinGame(gameId, joiner, out var state))
            {
                state = _gameService.CreateGame(gameId, joiner);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Group(gameId.ToString()).JoinedGame(state);
        }
        public async Task MakeMove(PlayerMoveInfo moveInfo)
        {
            var state = _gameService.MakeMove(moveInfo);
            await Clients.Group(moveInfo.GameId.ToString()).UpdateBoard(state);
        }
        public async Task GetMoves(AvailableMovesRequest request)
        {
            await Clients.Caller.ReceiveMoves(_gameService.GetAvailableMoves(request));
        }
    }
}
