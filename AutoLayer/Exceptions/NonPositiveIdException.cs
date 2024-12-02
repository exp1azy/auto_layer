namespace AutoLayer.Exceptions
{
    public class NonPositiveIdException(string error) : ErrorWithMessagesException(error)
    {
    }
}
