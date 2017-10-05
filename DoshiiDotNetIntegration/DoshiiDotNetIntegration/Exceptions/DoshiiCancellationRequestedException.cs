using System;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Exceptions
{


    /// <summary>
    /// Raised when an operation is requested after cancellation request.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class DoshiiCancellationRequestedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DoshiiCancellationRequestedException()
        {
        }

        public DoshiiCancellationRequestedException(string message) : base(message)
        {
        }

        public DoshiiCancellationRequestedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DoshiiCancellationRequestedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}