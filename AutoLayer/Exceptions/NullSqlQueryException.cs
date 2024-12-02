namespace AutoLayer.Exceptions
{
    public class NullSqlQueryException(string error) : ErrorWithMessagesException(error)
    {
    }
}
