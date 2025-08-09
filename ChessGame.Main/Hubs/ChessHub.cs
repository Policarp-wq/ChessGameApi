using ChessGame.Database.Models;
using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.GamePhysics;
using ChessGame.Main.Abstractions;
using ChessGame.Main.DTOs;
using ChessGame.Main.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChessGame.Main.Hubs
{
    public interface IChessHubClient
    {
        Task UpdateBoard(GameStateDTO state);
        Task JoinedGame(GameStateDTO state);

        //Task GameCreated(GameState state);
        Task ReceiveGameInviteCode(Guid gameCode);
        Task ReceiveMoves(List<ChessLocation> locations);
    }

    public class ChessHub : Hub<IChessHubClient>
    {
        private readonly IGameService _gameService;
        private readonly ILogger<ChessHub> _logger;

        public ChessHub(IGameService gameService, ILogger<ChessHub> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public async Task CreateGame(User user)
        {
            var gameId = _gameService.CreateGameRequest(user);
            _logger.LogInformation(
                "Game created with ID: {GameId} by user with id: {User}",
                gameId,
                user.Id
            );
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            _logger.LogInformation("Send game invite code to user: {UserId}", user.Id);
            await Clients.Caller.ReceiveGameInviteCode(gameId);
        }

        public async Task JoinGame(Guid gameId, User joiner)
        {
            if (!_gameService.TryJoinGame(gameId, joiner, out var state))
            {
                state = _gameService.CreateGame(gameId, joiner);
                _logger.LogInformation(
                    "Game with ID: {GameId} created for: {User1} is {Side1}; {User2} is {Side2}",
                    gameId,
                    state.Player1.Id,
                    Enum.GetName(state.Player1.ChessSide),
                    state.Player2.Id,
                    Enum.GetName(state.Player2.ChessSide)
                );
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Group(gameId.ToString()).JoinedGame(state);
        }

        public async Task MakeMove(PlayerMoveInfo moveInfo)
        {
            var state = _gameService.MakeMove(moveInfo);
            _logger.LogInformation(
                "Player {PlayerId} made a move in game {GameId}",
                moveInfo.PlayerId,
                moveInfo.GameId
            );
            await Clients.Group(moveInfo.GameId.ToString()).UpdateBoard(state);
            _logger.LogInformation("Board updated for game: {GameId}", moveInfo.GameId);
        }

        public async Task GetMoves(AvailableMovesRequest request)
        {
            _logger.LogInformation(
                "Getting available moves for game: {GameId} and player: {Player}",
                request.GameId,
                request.PlayerId
            );
            var moves = _gameService.GetAvailableMoves(request);
            await Clients.Caller.ReceiveMoves(moves);
            _logger.LogInformation(
                "Available moves sent to player: {PlayerId} with count: {Count}",
                request.PlayerId,
                moves.Count
            );
        }
    }
}
