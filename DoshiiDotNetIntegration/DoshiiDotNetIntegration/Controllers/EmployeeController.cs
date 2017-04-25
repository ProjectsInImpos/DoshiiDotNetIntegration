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
    internal class EmployeeController
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
        internal EmployeeController(Models.ControllersCollection controllerCollection, HttpController httpComs)
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

        internal Employee GetEmployee(string doshiiId)
        {
            try
            {
                return _httpComs.GetEmployee(doshiiId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal IEnumerable<Employee> GetEmployees()
        {
            try
            {
                return _httpComs.GetEmployees();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal Employee SaveEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Id))
            {
                try
                {
                    return _httpComs.PostEmployee(employee);
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
            }
            else
            {
                try
                {
                    return _httpComs.PutEmployee(employee);
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
            }
        }

        internal bool DeleteEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Id))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, "you are attempting to delete an employee without a doshii Id.");
                return false;
            }
            else
            {
                try
                {
                    return _httpComs.DeleteEmployee(employee);
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
            }
        }
    }
}
