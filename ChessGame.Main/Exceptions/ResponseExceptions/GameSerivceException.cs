namespace ChessGame.Main.Exceptions.ResponseExceptions
{
    public class GameServiceException : ServerResponseException
    {
        public GameServiceException(string? message)
            : base(StatusCodes.Status400BadRequest, message) { }
    }
}
