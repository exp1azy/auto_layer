namespace AutoLayer.Exceptions
{
    public class ExecuteSqlRawException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
