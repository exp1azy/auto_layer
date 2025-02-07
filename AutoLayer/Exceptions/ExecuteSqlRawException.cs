namespace AutoLayer.Exceptions
{
    public class ExecuteSqlRawException : ErrorWithMessagesException
    {
        public ExecuteSqlRawException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
