namespace AutoLayer.Exceptions
{
    public class ErrorWithMessagesException(string error, params string[] messages) : Exception(string.Format(error, messages))
    {
    }
}
