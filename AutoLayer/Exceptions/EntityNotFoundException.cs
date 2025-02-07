namespace AutoLayer.Exceptions
{
    public class EntityNotFoundException : ErrorWithMessagesException
    {
        public EntityNotFoundException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
