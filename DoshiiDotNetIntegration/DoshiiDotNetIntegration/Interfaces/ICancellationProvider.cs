using System.Threading;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// Cancellation provider helps to signal cancellation to any on going iterative doshii actions eg. Member Sync
    /// <para>eg : Create a class Implementing ICancellationProvider with System.Threading.CancellationTokenSource and return System.Threading.CancellationTokenSource.Token to ICancellationProvider.IsCancellationRequested</para></summary>
    public interface ICancellationProvider
    {

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancellation requested.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>.
        /// </value>
        bool IsCancellationRequested { get; }

        
    }
}
