using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// this class is used internally by the SDK to run the bl related to tables. 
    /// NOTE: there are some methods involving table in the <see cref="ReservationController"/> and the <see cref="CheckinController"/>
    /// this class is used to hold bl that relates to tables that is not related to reservations or checkins. 
    /// </summary>
    internal class TableController
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
        internal TableController(Models.ControllersCollection controllerCollection, HttpController httpComs)
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

        internal virtual Table GetTable(string tableName)
        {
            try
            {
                return _httpComs.GetTable(tableName);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual List<Table> GetTables()
        {
            try
            {
                return _httpComs.GetTables().ToList();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual Table CreateTable(Table table)
        {
            try
            {
                return _httpComs.PostTable(table);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual Table UpdateTable(Table table, string oldTableName)
        {
            try
            {
                return _httpComs.PutTable(table, oldTableName);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual Table DeleteTable(string tableName)
        {
            try
            {
                return _httpComs.DeleteTable(tableName);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual List<Table> ReplaceTableListOnDoshii(List<Table> tableList)
        {
            try
            {
                return _httpComs.PutTables(tableList);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic SetTableAllocationWithoutCheckin(string posOrderId, List<string> tableNames, int covers)
        {
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos Allocating table '{0}' to Order '{1}'", tableNames[0], posOrderId));
            var actionResult = new ActionResultBasic();
            Order order = null;
            try
            {
                order = _controllersCollection.OrderingManager.RetrieveOrder(posOrderId);
                order.Version = _controllersCollection.OrderingManager.RetrieveOrderVersion(posOrderId);
                order.CheckinId = _controllersCollection.OrderingManager.RetrieveCheckinIdForOrder(posOrderId);
                order.Status = "accepted";
            }
            catch (OrderDoesNotExistOnPosException dne)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, " Order does not exist on POS during table allocation");
                throw dne;
            }

            if (!string.IsNullOrEmpty(order.CheckinId))
            {
                return ModifyTableAllocation(order.CheckinId, tableNames, covers);
            }
            else
            {
                CheckinActionResult checkinCreateResult = null;
                try
                {
                    Checkin newCheckin = new Checkin();
                    newCheckin.TableNames = tableNames;
                    newCheckin.Covers = covers;
                    checkinCreateResult = _httpComs.PostCheckin(newCheckin);
                }
                catch (Exception ex)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown while attempting a table allocation Order.Id{0} : {1}", order.Id, ex));
                    throw new CheckinUpdateException(string.Format(" a exception was thrown during a attempting to create a checkin for Order.Id{0}", order.Id), ex);
                }
                if (checkinCreateResult.Success)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" Order found, allocating table now"));

                    order.CheckinId = checkinCreateResult.CheckinId;
                    OrderActionResult returnedOrderResult = _controllersCollection.OrderingController.UpdateOrder(order);
                    if (returnedOrderResult.Success)
                    {
                        actionResult.Success = true;
                    }
                    else
                    {
                        actionResult.Success = false;
                        actionResult.FailReason = returnedOrderResult.FailReason;
                    }
                }
            }
            return actionResult;
        }

        internal virtual ActionResultBasic ModifyTableAllocation(string checkinId, List<string> tableNames, int covers)
        {
            StringBuilder tableNameStringBuilder = new StringBuilder();
            for (int i = 0; i < tableNames.Count(); i++)
            {
                if (i > 0)
                {
                    tableNameStringBuilder.Append(", ");
                }
                tableNameStringBuilder.Append(tableNames[i]);
            }

            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos modifying table allocation table '{0}' to checkin '{1}'", tableNameStringBuilder, checkinId));

            //create checkin
            Checkin checkinCreateResult = null;
            try
            {
                Checkin newCheckin = new Checkin();
                newCheckin.TableNames = tableNames;
                newCheckin.Id = checkinId;
                newCheckin.Covers = covers;
                checkinCreateResult = _httpComs.PutCheckin(newCheckin);
                if (checkinCreateResult == null)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an error modifying a checkin through Doshii, modifying the table allocation could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown while attempting a table allocation  for checkin {0} : {1}", checkinId, ex));
                throw new CheckinUpdateException(string.Format(" a exception was thrown during a attempting a table allocaiton for for checkin {0}", checkinId), ex);
            }
            return true;
        }
    }
}
