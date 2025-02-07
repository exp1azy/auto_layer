namespace AutoLayer.Exceptions
{
    public class ErrorWithMessagesException : Exception
    {
        public ErrorWithMessagesException(string error, params string[] messages) : base(string.Format(error, messages))
        {
        }
    }
}
