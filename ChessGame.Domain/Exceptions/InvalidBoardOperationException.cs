using ChessGame.Domain.Exceptions;

namespace ChessGame.Domain.Exceptions
{
    public class InvalidBoardOperationException : ChessException
    {
        public InvalidBoardOperationException(string message)
            : base(message) { }
    }
}