namespace AutoLayer.Exceptions
{
    public class NullEntityInCollectionException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
