using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class ActionResultBasic
    {
        /// <summary>
        /// the result of the request
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// the reason the request failed
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        /// the status code of the response if relevant. 
        /// </summary>
        public HttpStatusCode responseStatusCode { get; set; }
    }
}
