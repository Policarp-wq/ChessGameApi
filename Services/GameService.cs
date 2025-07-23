using ChessGameApi.ApiContracts;
using ChessGameApi.Exceptions.Chess;
using ChessGameApi.Handlers;
using ChessGameApi.Models;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ChessGameApi.Services
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

        public GameState CreateGame(Guid gameId, User joiner)
        {
            if (!_queue.TryGetUserByGameId(gameId, out var requester))
                throw new GameSerivceException($"No game request with this id {gameId}");
            if(joiner.Id == requester.Id)
                throw new GameSerivceException($"Same user cant create the game");
            var (player1, player2) = CreatePlayers(requester, joiner);
            var game = new Game(player1, player2, gameId);
            if(!_games.TryAdd(gameId, game))
            {
                throw new GameSerivceException($"Game with id {gameId} is already exist");
            }
            return game.CurrentState;
        }
        public GameState JoinGame(Guid gameId, User joiner)
        {
            if (!_games.TryGetValue(gameId, out var game))
                throw new GameSerivceException($"No game with id {gameId}");
            if (!game.IsPlayer(joiner.Id))
                throw new GameSerivceException($"This user is not playing in this game");
            return game.CurrentState;
        }
        public bool TryJoinGame(Guid gameId, User joiner, [NotNullWhen(true)] out GameState? state)
        {
            state = null;
            try
            {
                state = JoinGame(gameId, joiner);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        private static (Player, Player) CreatePlayers(User firstPlayer, User secondPlayer)
        {
            int number = DecideStartingPlayerNumber();
            Player player1, player2;
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
                throw new GameSerivceException($"No game with id {requests.GameId}");
            return game.GetPossibleMoves(requests.From);
        }

        public GameState GetGameState(Guid GameId)
        {
            if (!_games.TryGetValue(GameId, out var game))
                throw new InvalidOperationException($"No game with id {GameId}");
            return game.CurrentState;
        }

        public GameState MakeMove(PlayerMoveInfo moveInfo)
        {
            if (!_games.TryGetValue(moveInfo.GameId, out var game))
                throw new InvalidOperationException($"No game with id {moveInfo.GameId}");
            return game.MakeMove(moveInfo.From, moveInfo.To, moveInfo.PlayerId);
        }
    }
}
