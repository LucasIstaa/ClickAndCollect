using System.Runtime.Serialization;

namespace ClickAndCollect.Models.Classes
{
    [Serializable]
    public class InvalidBoxesCountException : Exception
    {
        public InvalidBoxesCountException()
        {
        }

        public InvalidBoxesCountException(string message) : base(message)
        {
        }

        public InvalidBoxesCountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidBoxesCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
