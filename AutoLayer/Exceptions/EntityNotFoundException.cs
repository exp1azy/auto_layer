namespace AutoLayer.Exceptions
{
    public class EntityNotFoundException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
