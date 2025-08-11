using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;

namespace ChessGame.Testing;

public class GameStateTesting
{
    private static void CollectionsEqual<T>(ICollection<T> expected, ICollection<T> collection)
    {
        Assert.Equal(expected.Count, collection.Count);
        Assert.All(collection, p => expected.Contains(p));
    }

    private static (Player, Player) GetWhiteBlackPlayers() =>
        (new Player(1, "", ChessColors.White, []), new Player(2, "", ChessColors.Black, []));

    private static GameState MovesAreCorrect(
        ChessBoard board,
        ICollection<ChessLocation> expected,
        ChessLocation target
    )
    {
        var (white, black) = GetWhiteBlackPlayers();

        var state = new GameState(board, white, black, white);
        var moves = state.GetMoves(target);

        CollectionsEqual(expected, moves);

        return state;
    }

    [Fact]
    public void Pawn_HasOneForwardMove_FromInitialPosition()
    {
        var (white, black) = GetWhiteBlackPlayers();
        var board = new ChessBoard();
        var pawn = board.CreatePiece(ChessPieceNames.Pawn, ChessColors.White);
        board.GetCell(3, 3).SetPiece(pawn);
        var state = new GameState(board, white, black, white);

        var moves = state.GetMoves(3, 3);

        Assert.Single(moves);
        Assert.Contains(new ChessLocation(3, 4), moves);
    }

    [Fact]
    public void King_CanGoAnyDirection_WhenNoAttackers()
    {
        var (_, _) = GetWhiteBlackPlayers();
        var board = new ChessBoard();
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        List<ChessLocation> expected =
        [
            new(2, 4),
            new(3, 4),
            new(4, 4),
            new(4, 3),
            new(4, 2),
            new(3, 2),
            new(2, 2),
            new(2, 3),
        ];

        board.GetCell(3, 3).SetPiece(king);

        MovesAreCorrect(board, expected, new(3, 3));
    }

    [Fact]
    public void King_CanNotGo_OnEnemyAttack()
    {
        var (_, _) = GetWhiteBlackPlayers();
        var board = new ChessBoard();
        var knight = board.CreatePiece(ChessPieceNames.Knight, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        List<ChessLocation> expected =
        [
            new(3, 4),
            new(4, 3),
            new(4, 2),
            new(3, 2),
            new(2, 2),
            new(2, 3),
        ];

        board.GetCell(3, 3).SetPiece(king);
        board.GetCell(3, 6).SetPiece(knight);

        MovesAreCorrect(board, expected, new(3, 3));
    }

    [Fact]
    public void ChessPiece_HasToProtectTheKing_WhenAttacked()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        var allyRook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.White);
        List<ChessLocation> expectedKing =
        [
            new(2, 4),
            new(4, 4),
            new(4, 3),
            new(4, 2),
            new(2, 2),
            new(2, 3),
        ];
        List<ChessLocation> expectedRook = [new(3, 5)];

        board.GetCell(3, 3).SetPiece(king);
        board.GetCell(3, 6).SetPiece(rook);
        board.GetCell(7, 5).SetPiece(allyRook);

        MovesAreCorrect(board, expectedKing, new(3, 3));
        MovesAreCorrect(board, expectedRook, new(7, 5));
    }

    [Fact]
    public void King_CanMove_OnEnemy()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        List<ChessLocation> expectedKing = [new(6, 6)];

        board.GetCell(7, 7).SetPiece(king);
        board.GetCell(6, 6).SetPiece(rook);

        MovesAreCorrect(board, expectedKing, new(7, 7));
    }

    [Fact]
    public void Checkmate_WhenNowhere_ToMove()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        List<ChessLocation> expectedKing = [];

        board.GetCell(7, 7).SetPiece(king);
        board.GetCell(6, 5).SetPiece(rook);
        board.GetCell(5, 6).SetPiece(rook);

        var state = MovesAreCorrect(board, expectedKing, new(7, 7));
        Assert.True(state.IsOver);
    }

    [Fact]
    public void GameContinues_WhenKingCornered_ButSafe()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        var knight = board.CreatePiece(ChessPieceNames.Knight, ChessColors.White);
        List<ChessLocation> expectedKing = [];

        board.GetCell(7, 7).SetPiece(king);
        board.GetCell(6, 5).SetPiece(rook);
        board.GetCell(5, 6).SetPiece(rook);
        board.GetCell(4, 4).SetPiece(knight);

        var state = MovesAreCorrect(board, expectedKing, new(7, 7));
        Assert.False(state.IsOver);
    }

    [Fact]
    public void GameContinues_WhenAlly_BeatEnemy()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var queen = board.CreatePiece(ChessPieceNames.Queen, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        var knight = board.CreatePiece(ChessPieceNames.Knight, ChessColors.White);
        List<ChessLocation> expectedKing = [];
        List<ChessLocation> expectedKnight = [new(5, 7)];

        board.GetCell(7, 7).SetPiece(king);
        board.GetCell(5, 7).SetPiece(queen);
        board.GetCell(5, 6).SetPiece(rook);
        board.GetCell(4, 5).SetPiece(knight);

        var state = MovesAreCorrect(board, expectedKing, new(7, 7));
        Assert.False(state.IsOver);
        MovesAreCorrect(board, expectedKnight, new(4, 5));
    }
}