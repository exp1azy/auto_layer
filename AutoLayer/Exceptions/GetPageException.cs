namespace AutoLayer.Exceptions
{
    public class GetPageException(string error) : ErrorWithMessagesException(error)
    {
    }
}
