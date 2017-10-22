using DoshiiDotNetIntegration.Interfaces;

namespace DoshiiDotNetIntegration.Helpers
{
    /// <summary>
    /// Generic implementation of ICancellationProvider to avoid null check 
    /// </summary>
    /// <seealso cref="DoshiiDotNetIntegration.Interfaces.ICancellationProvider" />
    internal class GenericCancellationProvider : ICancellationProvider
    {


        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancellation requested.
        /// <para>Returns false always </para>
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>. 
        /// <para>Returns false by default</para>
        /// </value>
        public bool IsCancellationRequested
        {
            get { return false; }
        }
    }
}
