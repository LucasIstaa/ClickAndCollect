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

#pragma warning disable SYSLIB0051 // Constructeur requis par la convention [Serializable] classique, malgré l'obsolescence de l'API de sérialisation par formatteur.
        protected InvalidBoxesCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#pragma warning restore SYSLIB0051
    }
}
