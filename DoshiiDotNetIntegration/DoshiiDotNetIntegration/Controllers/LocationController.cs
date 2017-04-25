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
        /// prop for the local <see cref="Controllers"/> instance. 
        /// </summary>
        internal Models.Controllers _controllers;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="httpComs"></param>
        internal LocationController(Models.Controllers controller, HttpController httpComs)
        {
            if (controller == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllers = controller;
            if (_controllers.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            if (httpComs == null)
            {
                _controllers.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
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
