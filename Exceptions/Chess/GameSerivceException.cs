namespace ChessGameApi.Exceptions.Chess
{
    public class GameSerivceException : ServerException
    {
        public GameSerivceException(string? message) : base(StatusCodes.Status400BadRequest, message)
        {
        }
    }
}
