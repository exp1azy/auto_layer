namespace AutoLayer.Exceptions
{
    public class NullEntityInCollectionException : ErrorWithMessagesException
    {
        public NullEntityInCollectionException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
