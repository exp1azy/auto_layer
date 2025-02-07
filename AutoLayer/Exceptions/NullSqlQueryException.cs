namespace AutoLayer.Exceptions
{
    public class NullSqlQueryException : ErrorWithMessagesException
    {
        public NullSqlQueryException(string error) : base(error)
        {
        }
    }
}
