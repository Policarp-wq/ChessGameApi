using ChessGameApi.Exceptions.Chess;
using ChessGameApi.Models.ChessPieces;

namespace ChessGameApi.Models
{
    public sealed class ChessBoard
    {
        public const int DIM_X = 7;
        public const int DIM_Y = 7;
        private BoardCell[,] Cells;
        public ChessBoard()
        {
            //use object pool for locations
            Cells = new BoardCell[DIM_Y + 1, DIM_X + 1];
            for(int i = 0; i <= DIM_Y; ++i)
            {
                for(int j = 0; j <= DIM_X; ++j)
                {
                    Cells[i, j] = new BoardCell(i, j);
                }
            }
        }
        public void Clear()
        {
            for (int i = 0; i <= DIM_Y; ++i)
            {
                for (int j = 0; j <= DIM_X; ++j)
                {
                    Cells[i, j].Clear();
                }
            }
        }
        public void MovePiece(BoardCell from, BoardCell to)
        {
            if (from.Piece == null)
                return;
            to.Clear();
            to.SetPiece(from.Piece);
            to.Piece!.OnMoved(to);
            from.Clear();
        }
        public void FillBoardWithInitialState()
        {
            PieceInfo?[,] names = new PieceInfo?[DIM_Y + 1, DIM_X + 1];
 
            names[0, 0] = new("Rook", 'w');
            names[0, 1] = new("Knight", 'w');
            names[0, 2] = new("Bishop", 'w');
            names[0, 3] = new("Queen", 'w');
            names[0, 4] = new("King", 'w');
            names[0, 5] = new("Bishop", 'w');
            names[0, 6] = new("Knight", 'w');
            names[0, 7] = new("Rook", 'w');

            for (int i = 0; i <= DIM_X; i++)
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

            for (int i = 0; i <= DIM_X; i++)
            {
                names[6, i] = new("Pawn", 'b');
            }

            FillBoard(names);
        }
        public void FillBoard(PieceInfo?[,] names)
        {
            Clear();
            if (!(names.GetLength(0) == Cells.GetLength(0) && names.GetLength(1) != Cells.GetLength(1)))
                throw new InvalidBoardOperationException($"Attempted to fill board with matrix dimension different: was {names.GetLength(0)}, {names.GetLength(1)}");
            for(int i = 0; i < names.GetLength(0); ++i)
            {
                for(int j = 0; j < names.GetLength(1); ++j)
                {
                    if (names[i, j] != null)
                        Cells[i, j].SetPiece(CreatePiece(names[i, j]!.Name, names[i, j]!.Color));
                }
            }
        }
        public ChessPiece CreatePiece(ChessPieceNames name, ChessColors color)
        =>
            name switch
            {
                ChessPieceNames.Pawn => new Pawn(color),
                ChessPieceNames.Rook => new Rook(color),
                ChessPieceNames.Bishop => new Bishop(color),
                ChessPieceNames.Knight => new Knight(color),
                ChessPieceNames.Queen => new Queen(color),
                ChessPieceNames.King => new King(color),
                _ => throw new ArgumentException($"{Enum.GetName(name)} cannot be created: didn't specify constructor")
            };
  
        
        public BoardCell? TryGetCell(int x, int y)
        {
            if(y > DIM_Y || y < 0 || x > DIM_X || x < 0)
                return null;
            return Cells[x, y];
        }
        public BoardCell GetCell(int x, int y)
        {
            return Cells[x, y];
        }
        public List<ChessLocation> GetPossibleMoves(int x, int y)
        {
            var cell = TryGetCell(x, y);
            if (cell == null)
                throw new InvalidBoardOperationException($"Attempted to get moves from outside the board: {x} {y}");
            if (cell.Piece == null)
                return [];
            return cell.Piece.GetPossibleMoves(this, cell);
        }
    }
}
