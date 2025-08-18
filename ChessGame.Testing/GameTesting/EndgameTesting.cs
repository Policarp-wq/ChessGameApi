using System;
using ChessGame.Domain.GamePhysics;
using ChessGame.Domain.Gameplay;
using Xunit.Sdk;

namespace ChessGame.Testing.GameTesting;

public class EndgameTesting
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
    public void WhenGame_IsOver_ResultIsNotNull()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        var (white, black) = GetWhiteBlackPlayers();

        board.GetCell(0, 0).SetPiece(king);
        board.GetCell(0, 5).SetPiece(rook);
        board.GetCell(1, 5).SetPiece(rook);

        var state = new GameState(board, white, black, white);

        Assert.True(state.IsOver);
        Assert.NotNull(state.Result);
    }

    [Fact]
    public void Checkmate_WhenNowhere_ToMove_AndKing_IsUnderAttack()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        var (white, black) = GetWhiteBlackPlayers();

        board.GetCell(0, 0).SetPiece(king);
        board.GetCell(0, 5).SetPiece(rook);
        board.GetCell(1, 5).SetPiece(rook);

        var state = new GameState(board, white, black, white);

        Assert.True(state.IsKingUnderAttack);
        Assert.NotNull(state.Result);
        Assert.Equal(EndgameState.Checkmate, state.Result.State);
    }

    [Fact]
    public void Checkmate_WhenKing_IsUnderAttack_AndCantEscape_ButOtherPiece_Exists()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var knight = board.CreatePiece(ChessPieceNames.Rook, ChessColors.White);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        var (white, black) = GetWhiteBlackPlayers();

        board.GetCell(0, 0).SetPiece(king);
        board.GetCell(0, 5).SetPiece(rook);
        board.GetCell(1, 5).SetPiece(rook);

        board.GetCell(7, 7).SetPiece(knight);

        var state = new GameState(board, white, black, white);

        Assert.True(state.IsKingUnderAttack);
        Assert.NotNull(state.Result);
        Assert.Equal(EndgameState.Checkmate, state.Result.State);
    }

    [Fact]
    public void Stalemate_WhenNowhere_ToMove_AndKing_IsSafe()
    {
        var board = new ChessBoard();
        var rook = board.CreatePiece(ChessPieceNames.Rook, ChessColors.Black);
        var king = board.CreatePiece(ChessPieceNames.King, ChessColors.White);
        List<ChessLocation> expectedKing = [];

        board.GetCell(7, 7).SetPiece(king);
        board.GetCell(6, 5).SetPiece(rook);
        board.GetCell(5, 6).SetPiece(rook);

        var state = MovesAreCorrect(board, expectedKing, new(7, 7));

        Assert.False(state.IsKingUnderAttack);
        Assert.NotNull(state.Result);
        Assert.Equal(EndgameState.Stalemate, state.Result.State);
    }

    [Fact]
    public void GameContinues_WhenKingCornered_ButSafe_AndMovesAvailable()
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

        Assert.False(state.IsKingUnderAttack);
        Assert.NotEmpty(board.GetPossibleMoves(4, 4));
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
