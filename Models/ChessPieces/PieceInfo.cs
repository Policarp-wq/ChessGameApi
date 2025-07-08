namespace ChessGameApi.Models.ChessPieces
{
    public record PieceInfo(ChessPieceNames Name, ChessColors Color)
    {
        // Constructor with parameters must call the primary constructor using "this" initializer.  
        public PieceInfo(string name, char color)
            : this(Enum.Parse<ChessPieceNames>(name), color == 'w' ? ChessColors.White : ChessColors.Black)
        {
        }
    }
}
