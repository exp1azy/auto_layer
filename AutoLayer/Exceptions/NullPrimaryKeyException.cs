namespace AutoLayer.Exceptions
{
    public class NullPrimaryKeyException : ErrorWithMessagesException
    {
        public NullPrimaryKeyException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
