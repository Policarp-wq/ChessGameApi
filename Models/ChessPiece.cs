using ChessGameApi.Models.Gameplay;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ChessGameApi.Models
{
    public abstract class ChessPiece
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChessColors Color { get; private set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChessPieceNames Name { get; private set; }

        public ChessPiece(ChessColors color, ChessPieceNames name)
        {
            Color = color;
            Name = name;
        }
        public virtual void OnMoved(BoardCell target)
        {

        }
        public abstract List<ChessLocation> GetPossibleMoves(MoveContext context);
        protected bool TryMove(ChessBoard board, int x, int y,[NotNullWhen(true)] out BoardCell? cell)
        {
            cell = board.TryGetCell(x, y);
            if (cell == null)
                return false;
            if (cell.Piece != null && cell.Piece.Color == Color)
                return false;
            return true;
        }
        protected bool CanAttack(BoardCell? cell)
        {
            if (cell == null) return false;
            return cell.Piece != null && cell.Piece.Color != Color;
        }
        public override bool Equals(object? obj)
        {
            if(obj is not ChessPiece piece) return false;
            return piece.Color == Color && piece.Name == Name;  
        }
    }
}
