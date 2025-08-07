namespace ChessGame.Main.Exceptions
{
    public class GameServiceException : ServerException
    {
        public GameServiceException(string? message)
            : base(StatusCodes.Status400BadRequest, message) { }
    }
}
