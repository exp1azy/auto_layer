namespace AutoLayer.Exceptions
{
    public class NonPositiveIdException : ErrorWithMessagesException
    {
        public NonPositiveIdException(string error) : base(error)
        {
        }
    }
}
