using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Helpers
{
    internal static class DoshiiStrings
    {
        public static string GetUnknownErrorString(string errorProcess)
        {
            return string.Format("An unknown error occured attempting {0}", errorProcess);
        }

        public static string DoshiiLogPrefix = "Doshii:";
    }
}
