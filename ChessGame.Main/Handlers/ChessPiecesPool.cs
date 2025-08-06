using ChessGameApi.Models;
using ChessGameApi.Models.ChessPieces;

namespace ChessGameApi.Handlers
{
    public static class ChessPiecesPool
    {
        private static readonly Dictionary<
            ChessPieceNames,
            (ChessPiece White, ChessPiece Black)
        > _pool;

        static ChessPiecesPool()
        {
            _pool = [];
            FillPool();
        }

        private static void FillPool()
        {
            foreach (var val in Enum.GetValues<ChessPieceNames>())
            {
                _pool.Add(
                    val,
                    new(CreatePiece(val, ChessColors.White), CreatePiece(val, ChessColors.Black))
                );
            }
        }

        public static ChessPiece GetChessPiece(ChessPieceNames name, ChessColors color)
        {
            if (_pool.TryGetValue(name, out var pair))
            {
                return color switch
                {
                    ChessColors.White => pair.White,
                    ChessColors.Black => pair.Black,
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(color),
                        $"Unknown color: {color}"
                    ),
                };
            }
            throw new ArgumentException($"No piece with name {Enum.GetName(name)}");
        }

        private static ChessPiece CreatePiece(ChessPieceNames name, ChessColors color) =>
            name switch
            {
                ChessPieceNames.Pawn => new Pawn(color),
                ChessPieceNames.Rook => new Rook(color),
                ChessPieceNames.Bishop => new Bishop(color),
                ChessPieceNames.Knight => new Knight(color),
                ChessPieceNames.Queen => new Queen(color),
                ChessPieceNames.King => new King(color),
                _ => throw new ArgumentException(
                    $"{Enum.GetName(name)} cannot be created: didn't specify constructor"
                ),
            };
    }
}
