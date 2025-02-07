namespace AutoLayer.Exceptions
{
    public class GetPageException : ErrorWithMessagesException
    {
        public GetPageException(string error) : base(error)
        {
        }
    }
}
