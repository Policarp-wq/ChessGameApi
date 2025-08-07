using ChessGame.Domain.Exceptions;

namespace ChessGame.Domain.Exceptions
{
    public class GameQueueException : ChessException
    {
        public GameQueueException(string? message)
            : base(message) { }
    }
}
