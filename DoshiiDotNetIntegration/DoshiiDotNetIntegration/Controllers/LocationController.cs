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
    internal class LocationController
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
        internal LocationController(Models.ControllersCollection controllerCollection, HttpController httpComs)
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
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        public virtual Location GetLocation()
        {
            try
            {
                return _httpComs.GetLocation();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public virtual Location GetLocation(string hashedLocationId)
        {
            try
            {
                return _httpComs.GetLocation(hashedLocationId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Location CreateLocation(Location location)
        {
            try
            {
                return _httpComs.PostLocation(location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<Location> GetLocations()
        {
            try
            {
                return _httpComs.GetLocations();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Orginisation CreateOrginisation(Orginisation orginisation)
        {
            try
            {
                return _httpComs.PostOrginisation(orginisation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
