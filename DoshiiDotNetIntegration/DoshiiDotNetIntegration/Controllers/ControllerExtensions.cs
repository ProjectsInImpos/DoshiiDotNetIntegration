using System.Runtime.CompilerServices;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// Extension methods for controller collection
    /// </summary>
    internal static class ControllerExtensions
    {
        /// <summary>
        /// Determines whether [is cancellation requested] [the specified from].
        /// </summary>
        /// <param name="controllersCollection">The controllers collection.</param>
        /// <param name="from">From.</param>
        /// <returns>
        ///   <c>true</c> if [is cancellation requested] [the specified from]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCancellationRequested(this ControllersCollection controllersCollection,
            [CallerMemberName] string from = "")
        {
            if (controllersCollection.CancellationProvider.IsCancellationRequested &&
                controllersCollection.LoggingController != null)
            {
                controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug,
                    string.Format("Returning form function '{0}' due to pending cancellation request", from));
            }
            return controllersCollection.CancellationProvider.IsCancellationRequested;

        }
    }
}
