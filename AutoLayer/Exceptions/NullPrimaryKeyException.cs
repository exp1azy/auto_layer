namespace AutoLayer.Exceptions
{
    public class NullPrimaryKeyException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
