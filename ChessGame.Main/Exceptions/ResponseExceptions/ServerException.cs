using System.Net;

namespace ChessGame.Main.Exceptions.ResponseExceptions
{
    public class ServerResponseException : ApplicationException
    {
        public readonly int Code;

        public ServerResponseException(HttpStatusCode code, string? message)
            : this((int)code, message) { }

        public ServerResponseException(int code, string? message)
            : base(message)
        {
            Code = code;
        }
    }
}
