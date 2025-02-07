namespace AutoLayer.Exceptions
{
    public class NullEntityException : ErrorWithMessagesException
    {
        public NullEntityException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
