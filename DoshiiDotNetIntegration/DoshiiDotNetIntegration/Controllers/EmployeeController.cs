using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;

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
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        internal ObjectActionResult<Employee> GetEmployee(string doshiiId)
        {
            try
            {
                return _httpComs.GetEmployee(doshiiId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal ObjectActionResult<List<Employee>> GetEmployees()
        {
            try
            {
                return _httpComs.GetEmployees();
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal ObjectActionResult<Employee> SaveEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Id))
            {
                try
                {
                    return _httpComs.PostEmployee(employee);
                }
                catch (Exception rex)
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
                catch (Exception rex)
                {
                    throw rex;
                }
            }
        }

        internal ActionResultBasic DeleteEmployee(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, DoshiiStrings.GetAttemptingActionWithEmptyId("delete an employee", "employee"));
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = "CheckinId was empty"
                };
            }
            else
            {
                try
                {
                    return _httpComs.DeleteEmployee(employeeId);
                }
                catch (Exception rex)
                {
                    throw rex;
                }
            }
        }
    }
}
