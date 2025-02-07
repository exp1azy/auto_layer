namespace AutoLayer.Exceptions
{
    public class TransactionException : ErrorWithMessagesException
    {
        public TransactionException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
