namespace ChessGameApi.Exceptions.Chess
{
    public class GameServiceException : ServerException
    {
        public GameServiceException(string? message)
            : base(StatusCodes.Status400BadRequest, message) { }
    }
}
