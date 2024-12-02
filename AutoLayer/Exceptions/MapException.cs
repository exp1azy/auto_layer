namespace AutoLayer.Exceptions
{
    public class MapException(string error, params string[] messages) : ErrorWithMessagesException(error, messages)
    {
    }
}
