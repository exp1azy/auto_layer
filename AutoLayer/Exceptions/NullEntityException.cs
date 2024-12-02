namespace AutoLayer.Exceptions
{
    public class NullEntityException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
