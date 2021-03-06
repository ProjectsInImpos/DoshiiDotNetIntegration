﻿using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Interfaces;
using System;
using DoshiiDotNetIntegration.Helpers;

namespace DoshiiDotNetIntegration.Controllers
{
	/// <summary>
	/// This class is used internally by the SDK to manage the logging of messages back to the POS implementation.
	/// </summary>
	internal class LoggingController : IDisposable
	{
		/// <summary>
		/// A reference to the callback mechanism for message logging in the application.
		/// </summary>
		internal ILoggingManager mLog;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger">The callback for message logging in the application. Can optionally be <c>null</c> in
		/// which case messages logged by the SDK will not be returned to the application.</param>
		internal LoggingController(ILoggingManager logger)
		{
			mLog = logger;
		}

		#region IDisposable Members

		/// <summary>
		/// Cleanly disposes of the instance.
		/// </summary>
		public void Dispose()
		{
			mLog = null;
		}

		#endregion

		/// <summary>
		/// Logs a message to the application logging mechanism if available.
		/// </summary>
		/// <param name="type">The calling class type.</param>
		/// <param name="level">The logging level.</param>
		/// <param name="message">The raw string message to be logged.</param>
		/// <param name="ex">An optional exception associated with the log message.</param>
		internal virtual void LogMessage(Type type, DoshiiLogLevels level, string message, Exception ex = null)
		{
		    var messageString = string.Format("{0} {2}: {1}", DoshiiStrings.DoshiiLogPrefix, message,GetLogStringFromLogLevel(level));
            if (mLog != null)
		    {
                mLog.LogDoshiiMessage(type, level, messageString, ex);
		    }
				
		}

	    internal virtual void RecordSocketMessage(string messageType, string messageData)
	    {
	        if (mLog != null)
	        {
	            mLog.RecordSocketMessage(messageType, messageData);
	        }
	    }

        internal virtual string GetLogStringFromLogLevel(DoshiiLogLevels level)
        {
            string stringToReturn = "";
            switch (level)
	        {
	            case DoshiiLogLevels.Debug:
	                stringToReturn = "Debug";
	                break;
                case DoshiiLogLevels.Error:
	                stringToReturn = "Error";
	                break;
                case DoshiiLogLevels.Fatal:
	                stringToReturn = "Fatal";
	                break;
                case DoshiiLogLevels.Info:
	                stringToReturn = "Info";
	                break;
                case DoshiiLogLevels.Warning:
	                stringToReturn = "Warning";
	                break;
	        }
            return stringToReturn;
            
        }
	}
}
