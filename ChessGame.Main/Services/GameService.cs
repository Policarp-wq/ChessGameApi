using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
using ChessGame.Main.ApiContracts;
using ChessGame.Main.DTOs;
using ChessGame.Main.Exceptions;
using ChessGame.Main.Handlers;
using ChessGame.Main.Models;

namespace ChessGame.Main.Services
{
    public sealed class GameService : IGameService
    {
        //Игры не удаляются
        private readonly ConcurrentDictionary<Guid, Game> _games;
        private readonly GameQueue _queue;

        public GameService()
        {
            _games = new ConcurrentDictionary<Guid, Game>();
            _queue = new GameQueue();
        }

        public GameStateDTO CreateGame(Guid gameId, User joiner)
        {
            if (!_queue.TryGetUserByGameId(gameId, out var requester))
                throw new GameServiceException($"No game request with this id {gameId}");
            if (joiner.Id == requester.Id)
                throw new GameServiceException($"Same user cant create the game");
            var (player1, player2) = CreatePlayers(requester, joiner);
            var game = new Game(player1, player2, gameId);
            if (!_games.TryAdd(gameId, game))
            {
                throw new GameServiceException($"Game with id {gameId} is already exist");
            }
            return GameStateDTO.ToDTO(game.CurrentState);
        }

        public GameStateDTO JoinGame(Guid gameId, User joiner)
        {
            if (!_games.TryGetValue(gameId, out var game))
                throw new GameServiceException($"No game with id {gameId}");
            if (!game.IsPlayer(joiner.Id))
                throw new GameServiceException($"This user is not playing in this game");
            return GameStateDTO.ToDTO(game.CurrentState);
        }

        public bool TryJoinGame(
            Guid gameId,
            User joiner,
            [NotNullWhen(true)] out GameStateDTO? state
        )
        {
            state = null;
            try
            {
                state = JoinGame(gameId, joiner);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static (Player, Player) CreatePlayers(User firstPlayer, User secondPlayer)
        {
            int number = DecideStartingPlayerNumber();
            Player player1,
                player2;
            if (number == 1)
            {
                player1 = new Player(firstPlayer.Id, firstPlayer.Name, ChessColors.White, []);
                player2 = new Player(secondPlayer.Id, secondPlayer.Name, ChessColors.Black, []);
            }
            else
            {
                player1 = new Player(firstPlayer.Id, firstPlayer.Name, ChessColors.Black, []);
                player2 = new Player(secondPlayer.Id, secondPlayer.Name, ChessColors.White, []);
            }
            return (player1, player2);
        }

        private static int DecideStartingPlayerNumber()
        {
            return Random.Shared.Next(1, 3);
        }

        public Guid CreateGameRequest(User requester)
        {
            return _queue.AddUser(requester);
        }

        public List<ChessLocation> GetAvailableMoves(AvailableMovesRequest requests)
        {
            if (!_games.TryGetValue(requests.GameId, out var game))
                throw new GameServiceException($"No game with id {requests.GameId}");
            return game.GetPossibleMoves(requests.From);
        }

        public GameStateDTO GetGameState(Guid GameId)
        {
            if (!_games.TryGetValue(GameId, out var game))
                throw new InvalidOperationException($"No game with id {GameId}");
            return GameStateDTO.ToDTO(game.CurrentState);
        }

        public GameStateDTO MakeMove(PlayerMoveInfo moveInfo)
        {
            if (!_games.TryGetValue(moveInfo.GameId, out var game))
                throw new InvalidOperationException($"No game with id {moveInfo.GameId}");
            game.MakeMove(moveInfo.From, moveInfo.To, moveInfo.PlayerId);
            return GameStateDTO.ToDTO(game.CurrentState);
        }
    }
}
