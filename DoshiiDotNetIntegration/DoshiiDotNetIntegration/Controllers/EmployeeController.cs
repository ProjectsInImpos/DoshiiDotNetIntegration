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
        internal EmployeeController(Models.Controllers controller, HttpController httpComs)
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
                _controllers.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, "you are attempting to delete an employee without a doshii Id.");
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
