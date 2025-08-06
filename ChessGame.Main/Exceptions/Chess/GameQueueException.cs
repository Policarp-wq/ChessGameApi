namespace ChessGameApi.Exceptions.Chess
{
    public class GameQueueException : ServerException
    {
        public GameQueueException(string? message)
            : base(StatusCodes.Status400BadRequest, message) { }
    }
}
