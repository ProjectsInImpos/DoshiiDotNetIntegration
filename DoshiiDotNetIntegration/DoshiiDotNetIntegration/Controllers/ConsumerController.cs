using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// this class is used internally to control the bl for consumers. 
    /// NOTE: there are many consumer operations that are handled in the <see cref="OrderingController"/>
    /// this class handles all bl that is not related to ordering. 
    /// </summary>
    internal class ConsumerController
    {
        /// <summary>
        /// prop for the local <see cref="DoshiiDotNetIntegration.Controllers"/> instance. 
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
        internal ConsumerController(Models.ControllersCollection controllerCollection, HttpController httpComs)
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

        /// <summary>
        /// returns a consumer from the checkinId 
        /// </summary>
        /// <param name="checkinId"></param>
        /// <returns></returns>
        internal virtual Consumer GetConsumerFromCheckinId(string checkinId)
        {
            try
            {
                return _httpComs.GetConsumerFromCheckinId(checkinId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        
    }
}
