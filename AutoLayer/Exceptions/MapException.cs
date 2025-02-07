namespace AutoLayer.Exceptions
{
    public class MapException : ErrorWithMessagesException
    {
        public MapException(string error, params string[] messages) : base(error, messages)
        {
        }
    }
}
