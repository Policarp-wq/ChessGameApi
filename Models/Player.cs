namespace ChessGameApi.Models
{
    public record Player(int Id, string Name, ChessColors ChessSide, List<ChessPiece> Owns);
}
