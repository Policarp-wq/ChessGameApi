namespace ChessGameApi.Exceptions.Chess
{
    public class InvalidBoardOperationException : Exception
    {
        public InvalidBoardOperationException(string message) : base(message)
        {
        }
    }
}
