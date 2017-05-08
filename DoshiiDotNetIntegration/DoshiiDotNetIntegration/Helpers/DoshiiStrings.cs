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

        public static string DoshiiLogPrefix = "Doshii SDK:";

        public static string GetSuccessfulHttpResponseMessagesWithData(string httpMethod, string url, string data)
        {
            return string.Format("A {0} request to {1} returned a successful response with data", 
                httpMethod.ToUpperInvariant(), url);
        }


        public static string GetSuccessfulHttpResponseWithNoDataMessages(string httpMethod, string url)
        {
            return string.Format("A {0} request to {1} returned a successful response but there was not data contained in the response", 
                httpMethod.ToUpperInvariant(), url);
        }

        public static string GetUnsucessfulHttpResponseMessage(string httpMethod, string url)
        {
            return string.Format("A {0} request to {1} was not successful", 
                httpMethod.ToUpperInvariant(), url);
        }

        public static string GetNullHttpResponseMessage(string httpMethod, string url)
        {
            return string.Format("DoshiiHttpCommuication.MakeRequest returned NUll for a {0} request to {1}",
                httpMethod.ToUpperInvariant(), url);
        }

        public static string GetAttemptingActionWithEmptyId(string action, string modelRequiringId)
        {
            return string.Format("You have attempted to {0} with and empty {1} Id.", action, modelRequiringId);
        }

        public static string GetThereWasAnExceptionSeeLogForDetails(string action)
        {
            return string.Format("An exception occured attempting {0}, see log for details",action);
        }
    }
}
