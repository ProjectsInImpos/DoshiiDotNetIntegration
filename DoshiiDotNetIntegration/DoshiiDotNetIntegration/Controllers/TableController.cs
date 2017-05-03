using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;

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
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
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
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos Allocating table '{0}' to order '{1}'", tableNames[0], posOrderId));

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
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: Order does not exist on POS during table allocation");
                throw dne;
            }

            if (order == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: NULL Order returned from POS during table allocation");
                throw new OrderDoesNotExistOnPosException("Doshii: The pos returned a null order during table allocation", new NullResponseDataReturnedException());
            }

            if (!string.IsNullOrEmpty(order.CheckinId))
            {
                return ModifyTableAllocation(order.CheckinId, tableNames, covers);
            }

            //create checkin
            Checkin checkinCreateResult = null;
            try
            {
                Checkin newCheckin = new Checkin();
                newCheckin.TableNames = tableNames;
                newCheckin.Covers = covers;
                checkinCreateResult = _httpComs.PostCheckin(newCheckin);
                if (checkinCreateResult == null)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an error generating a new checkin through Doshii, the table allocation could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a table allocation order.Id{0} : {1}", order.Id, ex));
                throw new CheckinUpdateException(string.Format("Doshii: a exception was thrown during a attempting to create a checkin for order.Id{0}", order.Id), ex);
            }

            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: Order found, allocating table now"));

            order.CheckinId = checkinCreateResult.Id;
            Order returnedOrder = _controllersCollection.OrderingController.UpdateOrder(order);
            if (returnedOrder != null)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos modifying table allocation table '{0}' to checkin '{1}'", tableNameStringBuilder, checkinId));

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
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an error modifying a checkin through Doshii, modifying the table allocation could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a table allocation  for checkin {0} : {1}", checkinId, ex));
                throw new CheckinUpdateException(string.Format("Doshii: a exception was thrown during a attempting a table allocaiton for for checkin {0}", checkinId), ex);
            }
            return true;
        }
    }
}
