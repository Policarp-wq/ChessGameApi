using ChessGame.Database.Models;
using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.GamePhysics;
using ChessGame.Main.Abstractions;
using ChessGame.Main.DTOs;
using ChessGame.Main.Services;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    public class ChessHub(ILogger<ChessHub> _logger, IGameService _gameService)
        : Hub<IChessHubClient>
    {
        private PlayerRegisterInfo GetPlayerInfo()
        {
            var userLoginClaim = Context.User?.FindFirst(JwtService.UserLogin);
            var userIdClaim = Context.User?.FindFirst(JwtService.UserIdentifier);

            if (userLoginClaim == null || userIdClaim == null)
            {
                _logger.LogError("User claims not found.");
                throw new HubException("User claims not found.");
            }

            var userLogin = userLoginClaim.Value;
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogError("User id {Id} is not a number ", userIdClaim.Value);
                throw new HubException("User id is not a number");
            }
            return new(userId, userLogin);
        }

        public async Task CreateGame()
        {
            var playerInfo = GetPlayerInfo();
            var gameId = _gameService.CreateGameRequest(playerInfo);

            _logger.LogInformation(
                "Game created with ID: {GameId} by user with id: {User}",
                gameId,
                playerInfo.Id
            );
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            _logger.LogInformation("Send game invite code to user: {UserId}", playerInfo.Id);
            await Clients.Caller.ReceiveGameInviteCode(gameId);
        }

        public async Task JoinGame(Guid gameId)
        {
            var joiner = GetPlayerInfo();
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
