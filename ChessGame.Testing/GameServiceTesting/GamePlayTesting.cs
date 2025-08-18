using ChessGame.Database.Models;
using ChessGame.Domain.Exceptions;
using ChessGame.Domain.GamePhysics;
using ChessGame.Main.DTOs;
using ChessGame.Main.Services;

namespace ChessGame.Testing.GameServiceTesting
{
    public class GamePlayTesting
    {
        private static Guid CreateGame(GameService service)
        {
            var user1 = new PlayerRegisterInfo(1, "");
            var id = service.CreateGameRequest(user1);
            service.JoinByCodeAndCreateGame(id, new PlayerRegisterInfo(2, ""));
            return id;
        }

        private static (int, int) GetWhiteBlackPlayers(GameService service, Guid id)
        {
            var state = service.GetGameState(id);
            if (state.Player1.ChessSide == ChessColors.White)
                return (state.Player1.Id, state.Player2.Id);
            return (state.Player2.Id, state.Player1.Id);
        }

        [Fact]
        public void PlayersSides_AreDifferent()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);

            var state = gameService.GetGameState(id);

            Assert.False(state.Player1.ChessSide == state.Player2.ChessSide);
        }

        [Fact]
        public void WhiteSideIsPlaced_InTheStartOfBoard()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);

            var board = gameService.GetGameState(id).Board;

            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < ChessBoard.DIM_X; ++j)
                {
                    var cell = board.GetCell(j, i);
                    Assert.NotNull(cell.Piece);
                    Assert.Equal(ChessColors.White, cell.Piece.Color);
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < ChessBoard.DIM_X; ++j)
                {
                    var cell = board.GetCell(j, ChessBoard.DIM_Y - 1 - i);
                    Assert.NotNull(cell.Piece);
                    Assert.Equal(ChessColors.Black, cell.Piece.Color);
                }
            }
        }

        [Fact]
        public void InitialBoardMiddle_IsEmpty()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);

            var board = gameService.GetGameState(id).Board;
            for (int i = 2; i < ChessBoard.DIM_Y - 2; ++i)
            {
                for (int j = 0; j < ChessBoard.DIM_X; ++j)
                {
                    var cell = board.GetCell(j, i);
                    Assert.Null(cell.Piece);
                }
            }
        }

        private static ChessPieceNames GetPieceNameFromCell(ChessBoard board, int x, int y) =>
            board.GetCell(x, y).Piece!.Name;

        [Fact]
        public void InitialChessPieces_AreCorrect()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);
            var board = gameService.GetGameState(id).Board;

            List<ChessPieceNames> expectedRow =
            [
                ChessPieceNames.Rook,
                ChessPieceNames.Knight,
                ChessPieceNames.Bishop,
                ChessPieceNames.Queen,
                ChessPieceNames.King,
                ChessPieceNames.Bishop,
                ChessPieceNames.Knight,
                ChessPieceNames.Rook,
            ];

            for (int i = 0; i < ChessBoard.DIM_X; ++i)
            {
                Assert.Equal(expectedRow[i], GetPieceNameFromCell(board, i, 0));
                Assert.Equal(expectedRow[i], GetPieceNameFromCell(board, i, ChessBoard.DIM_Y - 1));
            }
            for (int i = 0; i < ChessBoard.DIM_X; ++i)
            {
                Assert.Equal(ChessPieceNames.Pawn, GetPieceNameFromCell(board, i, 1));
                Assert.Equal(
                    ChessPieceNames.Pawn,
                    GetPieceNameFromCell(board, i, ChessBoard.DIM_Y - 2)
                );
            }
        }

        private static GameStateDTO MakeMove(
            GameService service,
            Guid gameId,
            int playerId,
            int fromx,
            int fromy,
            int tox,
            int toy
        )
        {
            return GameStateDTO.ToDTO(
                service.MakeMove(
                    new(
                        gameId,
                        playerId,
                        new ChessLocation(fromx, fromy),
                        new ChessLocation(tox, toy)
                    )
                ),
                gameId
            );
        }

        [Fact]
        public void PawnsMovesCorrect()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);
            var (white, black) = GetWhiteBlackPlayers(gameService, id);
            var whitePawn = ChessPiecesPool.GetChessPiece(ChessPieceNames.Pawn, ChessColors.White);
            var blackPawn = ChessPiecesPool.GetChessPiece(ChessPieceNames.Pawn, ChessColors.Black);

            MakeMove(gameService, id, white, 0, 1, 0, 3);
            var after = MakeMove(gameService, id, black, 2, 6, 2, 4);
            Assert.Equal(whitePawn, after.Board.GetCell(0, 3).Piece);
            Assert.Equal(blackPawn, after.Board.GetCell(2, 4).Piece);
        }

        [Fact]
        public void ThrowsExceptionWhenMoveIllegal()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);
            var (white, black) = GetWhiteBlackPlayers(gameService, id);
            var whitePawn = ChessPiecesPool.GetChessPiece(ChessPieceNames.Pawn, ChessColors.White);
            var blackPawn = ChessPiecesPool.GetChessPiece(ChessPieceNames.Pawn, ChessColors.Black);

            Assert.Throws<InvalidBoardOperationException>(() =>
                MakeMove(gameService, id, white, 0, 1, 0, 4)
            );
        }

        [Fact]
        public void WhiteSide_MakesFirstMove()
        {
            var gameService = new GameService();
            var id = CreateGame(gameService);
            var (white, black) = GetWhiteBlackPlayers(gameService, id);

            var blackMoves = gameService.GetAvailableMoves(new(id, black, new(1, 6)));
            var whiteMoves = gameService.GetAvailableMoves(new(id, white, new(1, 1)));

            Assert.Empty(blackMoves);
            Assert.Equal(2, whiteMoves.Count);
        }
    }
}
