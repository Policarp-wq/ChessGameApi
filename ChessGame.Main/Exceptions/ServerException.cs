namespace ChessGameApi.Exceptions
{
    public class ServerException : Exception
    {
        public readonly int Code;

        public ServerException(int code, string? message)
            : base(message)
        {
            Code = code;
        }
    }
}
