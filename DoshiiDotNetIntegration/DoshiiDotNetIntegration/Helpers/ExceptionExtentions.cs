using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Exceptions;

namespace DoshiiDotNetIntegration.Helpers
{
    internal static class ExceptionExtentions
    {
        /// <summary>
        /// throws the DoshiiManagerNotInitializedException
        /// </summary>
        /// <param name="methodName">
        /// The name of the method that has been called before Initialize. 
        /// </param>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>

        internal static void ThrowDoshiiManagerNotInitializedException<T>(this T t, [CallerMemberName]string methodName = "")
        {
            throw new DoshiiManagerNotInitializedException(
                string.Format("You must initialize the DoshiiController instance before calling {0}.{1}", typeof(T), methodName));
        }

        internal static void ThrowDoshiiMembershipNotInitializedException<T>(this T t, [CallerMemberName]string methodName = "")
        {
            throw new DoshiiMembershipManagerNotInitializedException(
                string.Format("You must initialize the DoshiiMembership module before calling {0}.{1}", typeof(T), methodName));
        }
        
        internal static void ThrowDoshiiReservationNotInitializedException<T>(this T t, [CallerMemberName]string methodName = "")
        {
            throw new DoshiiMembershipManagerNotInitializedException(
                string.Format("You must initialize the DoshiiReservation module before calling {0}.{1}", typeof(T), methodName));
        }

        internal static void ThrowDoshiiAppNotInitializedException<T>(this T t, [CallerMemberName]string methodName = "")
        {
            throw new DoshiiMembershipManagerNotInitializedException(
                string.Format("You must initialize the DoshiiApp module before calling {0}.{1}", typeof(T), methodName));
        }

        internal static void ThrowDoshiiCancellationRequestedException<T>(this T t, [CallerMemberName]string methodName = "")
        {
            throw new DoshiiCancellationRequestedException(
                string.Format("Cancellation requested before executing method {0}.{1}", typeof(T), methodName));
        }

    }
}
