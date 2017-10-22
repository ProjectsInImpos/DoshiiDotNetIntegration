using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Interfaces;
using pos.Helpers;

namespace pos.DoshiiImplementation
{
    public class LoggingManager : ILoggingManager
    {
        public DisplayHelper DHelper;
        public LoggingManager(DisplayHelper dHelper)
        {
            DHelper = dHelper;
        }

        public void LogDoshiiMessage(Type callingClass, DoshiiLogLevels logLevel, string message, Exception ex = null)
        {
            DHelper.WriteToLog(message, logLevel);
        }

        public void RecordSocketMessage(string messageType, string messageData)
        {
            
        }
    }
}
