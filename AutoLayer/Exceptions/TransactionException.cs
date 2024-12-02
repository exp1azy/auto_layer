namespace AutoLayer.Exceptions
{
    public class TransactionException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
