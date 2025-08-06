using System.Text.Json.Serialization;
using ChessGameApi.Converters;
using ChessGameApi.Exceptions.Chess;
using ChessGameApi.Handlers;
using ChessGameApi.Models.ChessPieces;

namespace ChessGameApi.Models.Gameplay
{
    public sealed class ChessBoard : ICloneable
    {
        public const int DIM_X = 8;
        public const int DIM_Y = 8;

        [JsonConverter(typeof(Array2DConverter))]
        public BoardCell[,] Cells { get; private set; }

        public ChessBoard()
        {
            //use object pool for locations
            Cells = new BoardCell[DIM_Y, DIM_X];
            for (int i = 0; i < DIM_Y; ++i)
            {
                for (int j = 0; j < DIM_X; ++j)
                {
                    Cells[i, j] = new BoardCell(j, i);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < DIM_Y; ++i)
            {
                for (int j = 0; j < DIM_X; ++j)
                {
                    Cells[i, j].Clear();
                }
            }
        }

        public ChessPiece? MovePiece(BoardCell from, BoardCell to)
        {
            if (from.Piece == null)
                return null;
            var target = to.Piece;
            to.Clear();
            to.SetPiece(from.Piece);
            to.Piece!.OnMoved(to);
            from.Clear();
            return target;
        }

        public void FillBoardWithInitialState()
        {
            PieceInfo?[,] names = new PieceInfo?[DIM_Y, DIM_X];

            names[0, 0] = new("Rook", 'w');
            names[0, 1] = new("Knight", 'w');
            names[0, 2] = new("Bishop", 'w');
            names[0, 3] = new("Queen", 'w');
            names[0, 4] = new("King", 'w');
            names[0, 5] = new("Bishop", 'w');
            names[0, 6] = new("Knight", 'w');
            names[0, 7] = new("Rook", 'w');

            for (int i = 0; i < DIM_X; i++)
            {
                names[1, i] = new("Pawn", 'w');
            }

            names[7, 0] = new("Rook", 'b');
            names[7, 1] = new("Knight", 'b');
            names[7, 2] = new("Bishop", 'b');
            names[7, 3] = new("Queen", 'b');
            names[7, 4] = new("King", 'b');
            names[7, 5] = new("Bishop", 'b');
            names[7, 6] = new("Knight", 'b');
            names[7, 7] = new("Rook", 'b');

            for (int i = 0; i < DIM_X; i++)
            {
                names[6, i] = new("Pawn", 'b');
            }

            FillBoard(names);
        }

        public void FillBoard(PieceInfo?[,] names)
        {
            Clear();
            if (
                !(
                    names.GetLength(0) == Cells.GetLength(0)
                    && names.GetLength(1) == Cells.GetLength(1)
                )
            )
                throw new InvalidBoardOperationException(
                    $"Attempted to fill board with matrix dimension different: was {names.GetLength(0)}, {names.GetLength(1)}"
                );
            for (int i = 0; i < names.GetLength(0); ++i)
            {
                for (int j = 0; j < names.GetLength(1); ++j)
                {
                    if (names[i, j] != null)
                        Cells[i, j].SetPiece(CreatePiece(names[i, j]!.Name, names[i, j]!.Color));
                }
            }
        }

        public ChessPiece CreatePiece(ChessPieceNames name, ChessColors color) =>
            ChessPiecesPool.GetChessPiece(name, color);

        public BoardCell? TryGetCell(int x, int y)
        {
            if (y >= DIM_Y || y < 0 || x >= DIM_X || x < 0)
                return null;
            return GetCell(x, y);
        }

        public BoardCell? TryGetCell(ChessLocation location) => TryGetCell(location.X, location.Y);

        public BoardCell GetCell(int x, int y)
        {
            return Cells[y, x];
        }

        public BoardCell GetCell(ChessLocation location) => GetCell(location.X, location.Y);

        public List<ChessLocation> GetPossibleMoves(int x, int y)
        {
            var cell = TryGetCell(x, y);
            if (cell == null)
                throw new InvalidBoardOperationException(
                    $"Attempted to get moves from outside the board: {x} {y}"
                );
            if (cell.Piece == null)
                return [];
            return cell.Piece.GetPossibleMoves(new(this, cell));
        }

        public IEnumerable<BoardCell> GetCellsByPieceColor(ChessColors color)
        {
            for (int y = 0; y < DIM_Y; ++y)
            {
                for (int x = 0; x < DIM_X; ++x)
                {
                    var cell = GetCell(x, y);
                    if (cell.Piece != null && cell.Piece.Color == color)
                        yield return cell;
                }
            }
        }

        public List<ChessLocation> GetPossibleMoves(ChessLocation location) =>
            GetPossibleMoves(location.X, location.Y);

        public object Clone()
        {
            var board = new ChessBoard();
            for (int i = 0; i < board.Cells.GetLength(0); ++i)
            {
                for (int j = 0; j < board.Cells.GetLength(1); ++j)
                {
                    board.Cells[i, j] = (BoardCell)Cells[i, j].Clone();
                }
            }
            return board;
        }
    }
}
