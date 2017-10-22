using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;

namespace DoshiiDotNetIntegration.Controllers
{
    internal class RejectionCodeController
    {
        /// <summary>
        /// prop for the local <see cref="ControllersCollection"/> instance. 
        /// </summary>
        internal Models.ControllersCollection _controllersCollection;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controllerCollection"></param>
        /// <param name="httpComs"></param>
        internal RejectionCodeController(Models.ControllersCollection controllerCollection, HttpController httpComs)
        {
            if (controllerCollection == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllersCollection = controllerCollection;
            if (_controllersCollection.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        internal virtual ObjectActionResult<List<RejectionCode>> GetRejectionCodes()
        {
            try
            {
                return _httpComs.GetRejectionCodes();
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<RejectionCode> GetRejectionCode(string rejectionCodeId)
        {
            try
            {
                return _httpComs.GetRejectionCode(rejectionCodeId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
    }
}
