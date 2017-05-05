using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// This class is used internally by the SDK to manage the SDK to manage the business logic handling partner ordering. 
    /// </summary>
    internal class OrderingController
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
        internal OrderingController(Models.ControllersCollection controllerCollection, HttpController httpComs)
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
            if (_controllersCollection.OrderingManager == null)
            {

                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - IOrderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllersCollection.TransactionController == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - TransactionController cannot be null");
                throw new NullReferenceException("transactionController cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        /// <summary>
        /// calls the appropriate callback method on <see cref="Interfaces.IOrderingManager"/> to record the Order version.
        /// </summary>
        /// <param name="posOrderId">
        /// the PosId of the Order to be recorded
        /// </param>
        /// <param name="version">
        /// the version of the Order to be recorded.
        /// </param>
        internal virtual void RecordOrderVersion(string posOrderId, string version)
        {
            try
            {
                _controllersCollection.OrderingManager.RecordOrderVersion(posOrderId, version);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, string.Format(" Attempted to update an Order version that does not exist on the Pos, OrderId - {0}, version - {1}", posOrderId, version));
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" Exception while attempting to update an Order version on the pos, OrderId - {0}, version - {1}, {2}", posOrderId, version, ex.ToString()));
            }
        }

        /// <summary>
        /// This method returns an Order from Doshii corresponding to the OrderId
        /// </summary>
        /// <param name="orderId">
        /// The Id of the Order that is being requested. 
        /// </param>
        /// <returns>
        /// The Order with the corresponding Id
        /// <para/>If there is no Order corresponding to the Id, a blank Order may be returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an exception while making the request to doshii.</exception>
        internal virtual Models.Order GetOrder(string orderId)
        {
            try
            {
                return  PopupateAppIdPropsInOrder(_httpComs.GetOrder(orderId));
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual List<Log> GetOrderLog(Order order)
        {
            try
            {
                if (string.IsNullOrEmpty(order.Id))
                {
                    return _httpComs.GetUnlinkedOrderLog(order.DoshiiId);
                }
                else
                {
                    return _httpComs.GetOrderLog(order.Id);
                }
                
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns an Order from Doshii corresponding to the doshiiOrderId
        /// <para/>This will only return orders that are unlinked
        /// <para/>If doshii has already linked the Order on the pos then <see cref="GetOrder"/> should be called
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The Id of the Order that is being requested. 
        /// </param>
        /// <returns>
        /// The Order with the corresponding Id
        /// <para/>If there is no Order corresponding to the Id, a blank Order may be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        internal virtual OrderActionResult GetUnlinkedOrderFromDoshiiOrderId(string doshiiOrderId)
        {
            try
            {
                return PopupateAppIdPropsInOrder(_httpComs.GetOrderFromDoshiiOrderId(doshiiOrderId));
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns all Order for Doshii that have been provided with a PosId, 
        /// If the Order has not yet been processed by the pos and an Id has not been provided you should use <see cref="GetUnlinkedOrders"/> to retreive the Order. 
        /// </summary>
        /// <returns></returns>
        internal virtual IEnumerable<Models.Order> GetOrders()
        {
            try
            {
                return _httpComs.GetOrders();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// Retrieves the current unlinked Order list from Doshii.
        /// </summary>
        /// <returns>
        /// The current list of orders available in Doshii.
        /// <para/>If there are no unlinkedOrders a blank IEnumerable is returned.
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        internal virtual IEnumerable<Models.Order> GetUnlinkedOrders()
        {
            try
            {
                return _httpComs.GetUnlinkedOrders();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// attempt to update an Order on Doshii
        /// </summary>
        /// <param name="order">The Order to update.</param>
        /// <returns></returns>
        internal virtual OrderActionResult UpdateOrder(Models.Order order)
        {
            var actionResult = new OrderActionResult();
            order.Version = _controllersCollection.OrderingManager.RetrieveOrderVersion(order.Id);
            var jsonOrder = Mapper.Map<JsonOrder>(order);
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos updating Order - '{0}'", jsonOrder.ToJsonString()));

            var returnedOrder = new Models.Order();

            try
            {
                actionResult = _httpComs.PutOrder(order);
                if (actionResult.Order.Id == "0" && actionResult.Order.DoshiiId == "0")
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format(" Order was returned from doshii without an doshiiOrderId while updating Order with id {0}", order.Id));
                    throw new OrderUpdateException(string.Format(" Order was returned from doshii without an doshiiOrderId while updating Order with id {0}", order.Id));
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw new OrderUpdateException("Update Order not successful", rex);
            }
            catch (NullResponseDataReturnedException Nex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a Null response was returned during a putOrder for Order.Id{0}", order.Id));
                throw new OrderUpdateException(string.Format(" a Null response was returned during a putOrder for Order.Id{0}", order.Id), Nex);
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown during a putOrder for Order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format(" a exception was thrown during a putOrder for Order.Id{0}", order.Id), ex);
            }

            if (actionResult.Success)
            {
                return PopupateAppIdPropsInOrder(returnedOrder);
            }
            else
            {
                return actionResult;
            }
            
        }

        /// <summary>
        /// Confirms a pending Order on Doshii
        /// </summary>
        /// <param name="order">
        /// The Order to be confirmed. 
        /// </param>
        /// <returns></returns>
        internal virtual Models.Order PutOrderCreatedResult(Models.Order order)
        {
            if (order.Status == "accepted")
            {
                if (order.Id == null || string.IsNullOrEmpty(order.Id))
                {
                    throw new OrderUpdateException("the pos must set an Order.Id for accepted orders.");
                }
            }

            var returnedOrder = new Models.Order();

            try
            {
                returnedOrder = _httpComs.PutOrderCreatedResult(order);
                if (returnedOrder.Id == "0" && returnedOrder.DoshiiId == "0")
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format(" Order was returned from doshii without an doshiiOrderId while updating Order with id {0}", order.Id));
                    throw new OrderUpdateException(string.Format(" Order was returned from doshii without an doshiiOrderId while updating Order with id {0}", order.Id));
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.Conflict)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format("There was a conflict updating Order.id {0}", order.Id.ToString()));
                    throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating Order.id {0}", order.Id.ToString()));
                }
                throw new OrderUpdateException("Update Order not successful", rex);
            }
            catch (NullResponseDataReturnedException Nex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a Null response was returned during a putOrder for Order.Id{0}", order.Id));
                throw new OrderUpdateException(string.Format(" a Null response was returned during a putOrder for Order.Id{0}", order.Id), Nex);
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown during a putOrder for Order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format(" a exception was thrown during a putOrder for Order.Id{0}", order.Id), ex);
            }

            return PopupateAppIdPropsInOrder(returnedOrder);
        }

        /// <summary>
        /// Checks all orders on the doshii system, 
        /// <para/>if there are any pending orders <see cref="HandleOrderCreated"/> will be called. 
        /// <para/>currently this method does not check the transactions as there should be no unlinked transactions for already created orders, Order ahead only allows for 
        /// <para/>partners to make payments when they create an Order else the payment is expected to be made by the customer on receipt of the Order. 
        /// </summary>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown if there is an issue getting the orders from Doshii.</exception>
        internal virtual void RefreshAllOrders()
        {

            try
            {
                //check unassigned orders
                _controllersCollection.LoggingController.LogMessage(this.GetType(), DoshiiLogLevels.Info, "Refreshing all orders.");
                IEnumerable<Models.Order> unassignedOrderList;
                unassignedOrderList = GetUnlinkedOrders();
                foreach (Models.Order order in unassignedOrderList)
                {
                    if (order.Status == "pending")
                    {
                        List<Transaction> transactionListForOrder = _controllersCollection.TransactionController.GetTransactionFromDoshiiOrderId(order.DoshiiId).ToList();
                        HandleOrderCreated(order, transactionListForOrder.ToList());
                    }
                }
                //Check assigned orders
                //This is not yet implemented as its not necessary when only OrderAhead is a possibility. 
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method calls the appropriate callback method on the <see cref="Interfaces.IOrderingManager"/> to confirm an Order when an Order created event is received from Doshii. 
        /// </summary>
        /// <param name="order">
        /// the Order that has been created
        /// </param>
        /// <param name="transactionList">
        /// The transaction list for the new created Order. 
        /// </param>
        internal virtual void HandleOrderCreated(Models.Order order, List<Transaction> transactionList)
        {
            if (transactionList == null)
            {
                transactionList = new List<Transaction>();
            }
            if (order.Consumer == null)
            {
                _controllersCollection.LoggingController.LogMessage(this.GetType(), DoshiiLogLevels.Error, string.Format(" An Order created event was received with DoshiiId - {0} but the Order does not have a consumer, the Order has been rejected", order.DoshiiId));
                RejectOrderFromOrderCreateMessage(order, transactionList);
                return;
            }
            if (transactionList.Count > 0)
            {

                if (order.Type == "delivery")
                {
                    _controllersCollection.OrderingManager.ConfirmNewDeliveryOrderWithFullPayment(order, order.Consumer, transactionList);
                }
                else if (order.Type == "pickup")
                {
                    _controllersCollection.OrderingManager.ConfirmNewPickupOrderWithFullPayment(order, order.Consumer, transactionList);
                }
                else if (order.Type == "dinein")
                {
                    _controllersCollection.OrderingManager.ConfirmNewDineInOrderWithFullPayment(order, order.Consumer, transactionList);
                }
                else
                {
                    _controllersCollection.OrderingManager.ConfirmNewUnknownTypeOrderWithFullPayment(order, order.Consumer, transactionList);
                }
            }
            else
            {
                if (order.Type == "delivery")
                {

                    _controllersCollection.OrderingManager.ConfirmNewDeliveryOrder(order, order.Consumer);
                }
                else if (order.Type == "pickup")
                {
                    _controllersCollection.OrderingManager.ConfirmNewPickupOrder(order, order.Consumer);
                }
                else if (order.Type == "dinein")
                {
                    _controllersCollection.OrderingManager.ConfirmNewDineInOrder(order, order.Consumer);
                }
                else
                {
                    _controllersCollection.OrderingManager.ConfirmNewUnknownTypeOrder(order, order.Consumer);
                }

            }
        }

        /// <summary>
        /// This method calls the appropriate callback method on the <see cref="Interfaces.IOrderingManager"/> to confirm an Order when an Order created event is received from Doshii. 
        /// </summary>
        /// <param name="order">
        /// the Order that has been created
        /// </param>
        /// <param name="transactionList">
        /// The transaction list for the new created Order. 
        /// </param>
        internal virtual void HandleOrderUpdated(Models.Order order)
        {
            if (order.Status == "venue_cancelled")
            {
                _controllersCollection.OrderingManager.ProcessVenueCanceledOrderUpdate(order);
            }
            else
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, string.Format("A socket message was received from Doshii for an Order update, the Order status was {0}. This status is not currently supported for Order updates.", order.Status));
            }
            
        }

        /// <summary>
        /// Used to accept an Order created for a partner through the orderAhead interface. 
        /// </summary>
        /// <param name="orderToAccept"></param>
        /// <returns></returns>
        internal virtual OrderActionResult AcceptOrderAheadCreation(Models.Order orderToAccept)
        {
            Models.Order orderOnDoshii = GetUnlinkedOrderFromDoshiiOrderId(orderToAccept.DoshiiId);
            List<Transaction> transactionList = _controllersCollection.TransactionController.GetTransactionFromDoshiiOrderId(orderToAccept.DoshiiId).ToList();

            //test on doshii has changed. 
            if (orderOnDoshii.Version != orderToAccept.Version)
            {
                return false;
            }

            orderToAccept.Status = "accepted";
            try
            {
                PutOrderCreatedResult(orderToAccept);
            }
            catch (Exception ex)
            {
                return false;
                //although there could be an conflict exception from this method it is not currently possible for partners to update Order ahead orders so for the time being we don't need to handle it. 
                //if we get an error response at this point we should prob cancel the Order on the pos and not continue and cancel the payments. 
            }
            //If there are transactions set to waiting and get response - should call request payment
            foreach (Transaction tran in transactionList)
            {
                _controllersCollection.TransactionController.RecordTransactionVersion(tran);
                tran.OrderId = orderToAccept.Id;
                tran.Status = "waiting";
                try
                {
                    _controllersCollection.TransactionController.RequestPaymentForOrderExistingTransaction(tran);
                }
                catch (Exception ex)
                {
                    //although there could be an conflict exception from this method it is not currently possible for partners to update Order ahead orders so for the time being we don't need to handle it. 
                }
            }
            return true;
        }

        /// <summary>
        /// use to reject an Order created by a partner through the orderAhead interface. 
        /// </summary>
        /// <param name="orderToReject"></param>
        internal virtual void RejectOrderAheadCreation(Models.Order orderToReject)
        {
            List<Transaction> transactionList = _controllersCollection.TransactionController.GetTransactionFromDoshiiOrderId(orderToReject.DoshiiId).ToList();
            //test Order to accept is equal to the Order on doshii
            RejectOrderFromOrderCreateMessage(orderToReject, transactionList);
        }

        /// <summary>
        /// This method rejects an unlinked Order on the Doshii API, the transactions related to the Order will also be rejected. 
        /// </summary>
        /// <param name="order">
        /// The pending Order to be rejected
        /// </param>
        /// <param name="transactionList">
        /// The transaction list to be rejected
        /// </param>
        internal virtual void RejectOrderFromOrderCreateMessage(Models.Order order, List<Transaction> transactionList)
        {
            //set Order status to rejected post to doshii
            order.Status = "rejected";
            try
            {
                PutOrderCreatedResult(order);
            }
            catch (Exception ex)
            {
                //although there could be an conflict exception from this method it is not currently possible for partners to update Order ahead orders so for the time being we don't need to handle it. 
            }
            foreach (Transaction tran in transactionList)
            {
                tran.Status = "rejected";
                try
                {
                    _controllersCollection.TransactionController.RejectPaymentForOrder(tran);
                }
                catch (Exception ex)
                {
                    //although there could be an conflict exception from this method it is not currently possible for partners to update Order ahead orders so for the time being we don't need to handle it. 
                }
            }
        }

        /// <summary>
        /// Calls the appropriate callback method in <see cref="Interfaces.IOrderingManager"/> to record the checkinId for an Order on the pos. 
        /// </summary>
        /// <param name="order">
        /// The Order that need to be recorded. 
        /// </param>
        internal virtual void RecordOrderCheckinId(string posOrderId, string checkinId)
        {
            try
            {
                _controllersCollection.OrderingManager.RecordCheckinForOrder(posOrderId, checkinId);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format(" Attempted to update a checkinId for an Order that does not exist on the Pos, Order.id - {0}, checkinId - {1}", posOrderId, checkinId));
                //maybe we should call reject Order here. 
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" Exception while attempting to update a checkinId for an Order on the pos, Order.Id - {0}, checkinId - {1}, {2}", posOrderId, checkinId, ex.ToString()));
                //maybe we should call reject Order here. 
            }
        }

        internal virtual OrderActionResult PopupateAppIdPropsInOrder(OrderActionResult order)
        {
            return PopupateAppIdPropsInOrder(order.Order);
        }

        internal virtual OrderActionResult PopupateAppIdPropsInOrder(Order order)
        {
            var actionResult = new OrderActionResult()
            {
                Order = order,
                OrderId = order.Id
            };

            var logActionResult = new LogActionResult();
            if (order == null)
            {
                return null;
            }
            try
            {
                logActionResult = _httpComs.GetOrderLog(order.DoshiiId);
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(this.GetType(), DoshiiLogLevels.Warning, string.Format("There was an exception getting the logs for on Order with doshiiId = {0}. The partner Id will not be populated on this Order.", order.DoshiiId));
                return actionResult;
            }

            var orderLogForCreated = logActionResult.LogList.FirstOrDefault(l => l.Action == "order_created");
            if (orderLogForCreated != null)
            {
                order.OrderCreatedByAppId = orderLogForCreated.AppId;
            }
            var orderLogForUpdated = logActionResult.LogList.LastOrDefault(l => l.Action == "order_updated");
            if (orderLogForUpdated != null)
            {
                order.OrderCreatedByAppId = orderLogForUpdated.AppId;
            }

            return actionResult;
        }
    }
}
