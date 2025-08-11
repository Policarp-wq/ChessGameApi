namespace ChessGame.Domain.GamePhysics
{
    public enum ChessColors
    {
        White,
        Black,
    }

    public static class ChessColorsExtensions
    {
        public static ChessColors GetEnemy(this ChessColors color)
        {
            return color == ChessColors.White ? ChessColors.Black : ChessColors.White;
        }
    }
}