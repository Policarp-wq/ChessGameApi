using ChessGame.Database.Models;
using ChessGame.Domain.ChessPieces;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
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
        Task GameResumed(GameStateDTO state);
        Task GamePaused();
        Task GameOver(EndgameStats stats);
        Task Kick();
        Task UserLeft();
        Task GameCreated(GameStateDTO state);
        Task ReceiveGameInviteCode(Guid gameCode);
        Task ReceiveMoves(List<ChessLocation> locations);
    }

    [Authorize]
    public class ChessHub(ILogger<ChessHub> _logger, IGameService _gameService)
        : Hub<IChessHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            var player = GetPlayerInfoFromContext();
            await base.OnConnectedAsync();
            _logger.LogInformation("User {UserId} connected to the hub", player.Id);
            if (_gameService.TryRejoinGame(player.Id, out var gameId))
            {
                var group = gameId.Value.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, group);
                var gameDto = _gameService.GetGameState(gameId.Value);
                await Clients.Group(group).GameResumed(GameStateDTO.ToDTO(gameDto, gameId.Value));
                _logger.LogInformation(
                    "User {UserId} rejoined game {GameId}",
                    player.Id,
                    gameId.Value
                );
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var player = GetPlayerInfoFromContext();
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation("User {UserId} disconnected from the hub", player.Id);
            if (_gameService.TryLeaveGame(player.Id, out var gameId))
            {
                _logger.LogInformation("User {UserId} left game {GameId}", player.Id, gameId.Value);
                var group = gameId.Value.ToString();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
                if (_gameService.AreAllPlayersLeft(gameId.Value))
                {
                    _logger.LogInformation("All players left game {GameId}", gameId.Value);
                    FinishGameSession(gameId.Value);
                }
                else
                    await Clients.Group(group).GamePaused();
            }
        }

        private PlayerRegisterInfo GetPlayerInfoFromContext()
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

        public async Task CreateNewGameRequest()
        {
            var playerInfo = GetPlayerInfoFromContext();
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

        public async Task JoinNewGame(Guid gameId)
        {
            var playerInfo = GetPlayerInfoFromContext();
            var state = _gameService.JoinByCodeAndCreateGame(gameId, playerInfo);
            _logger.LogInformation(
                "Created game for players: {Id1} side {Side1} and {Id2} side {Side2}",
                state.Player1.Id,
                Enum.GetName(state.Player1.ChessSide),
                state.Player2.Id,
                Enum.GetName(state.Player2.ChessSide)
            );
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Group(gameId.ToString()).GameCreated(GameStateDTO.ToDTO(state, gameId));
        }

        private void FinishGameSession(Guid gameId)
        {
            _gameService.RemoveSession(gameId);
            _logger.LogInformation("Game session {GameId} removed", gameId);
        }

        private async Task EndGameForPlayers(Guid gameId, int winnerId)
        {
            var endgameStats = new EndgameStats(winnerId);
            _logger.LogInformation("Game {GameId} ended with winner {WinnerId}", gameId, winnerId);
            await Clients.Group(gameId.ToString()).GameOver(endgameStats);
            FinishGameSession(gameId);
        }

        public async Task MakeMove(PlayerMoveInfo moveInfo)
        {
            // todo: cannot make moves if game paused!
            var state = _gameService.MakeMove(moveInfo);
            _logger.LogInformation(
                "Player {PlayerId} made a move in game {GameId}",
                moveInfo.PlayerId,
                moveInfo.GameId
            );
            await Clients
                .Group(moveInfo.GameId.ToString())
                .UpdateBoard(GameStateDTO.ToDTO(state, moveInfo.GameId));
            _logger.LogInformation("Board updated for game: {GameId}", moveInfo.GameId);
            if (state.IsOver && state.WinnerId.HasValue)
                await EndGameForPlayers(moveInfo.GameId, state.WinnerId.Value);
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
