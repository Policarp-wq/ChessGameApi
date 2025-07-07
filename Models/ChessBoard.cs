namespace ChessGameApi.Models
{
    public sealed class ChessBoard
    {
        public const int MAX_X = 7;
        public const int MAX_Y = 7;
        private BoardCell[,] Cells;
        public ChessBoard()
        {
            //use object pool for locations
            Cells = new BoardCell[MAX_Y + 1, MAX_X + 1];
            for(int i = 0; i <= MAX_Y; ++i)
            {
                for(int j = 0; j <= MAX_X; ++j)
                {
                    Cells[i, j] = new BoardCell(i, j);
                }
            }
        }
        public BoardCell? GetCell(int x, int y)
        {
            if(y >= MAX_Y || y < 0 || x >= MAX_X || x < 0)
                return null;
            return Cells[x, y];
        }
    }
}
