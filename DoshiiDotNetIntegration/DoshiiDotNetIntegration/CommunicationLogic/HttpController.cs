using AutoMapper.Internal;
using AutoMapper.Mappers;
using AutoMapper;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using DoshiiDotNetIntegration.Controllers;
using JWT;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models.ActionResults;
using DoshiiDotNetIntegration.Models.Base;
using DoshiiDotNetIntegration.Models.Json.JsonBase;
using Newtonsoft.Json.Linq;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// This class is used internally by the SDK.
    /// This class manages the HTTP communications between the pos and the Doshii API.
    /// </summary>
    internal class HttpController 
    {
		/// <summary>
		/// The HTTP request method for a DELETE endpoint action.
		/// </summary>
		private const string DeleteMethod = "DELETE";

        /// <summary>
        /// The base URL for HTTP communication with Doshii,
        /// an example of the format for this URL is 'https://sandbox.doshii.co/pos/api/v2'
        /// </summary>
		internal string _doshiiUrlBase { get; private set; }

        internal Models.ControllersCollection _controllersCollection { get; set; }
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="urlBase">
        /// The base URL for HTTP communication with the Doshii API, <see cref="_doshiiUrlBase"/>
        /// </param>
		/// <param name="token">
		/// The Doshii pos token that will identify the pos on the Doshii API, <see cref="m_Token"/>
		/// </param>
		/// <param name="logManager">
		/// The <see cref="LoggingController"/> that is responsible for logging doshii messages, <see cref="LoggingController"/>
		/// </param>
        /// <param name="doshiiManager">
        /// the <see cref="DoshiiController"/> that controls the operation of the SDK.
        /// </param>
        internal HttpController(string urlBase, Models.ControllersCollection controllersCollection)
        {
            if (controllersCollection.LoggingController == null)
			{
				throw new ArgumentNullException("logManager");
			}
            if (controllersCollection.ConfigurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }
            _controllersCollection = controllersCollection;
            
            _controllersCollection.LoggingController.LogMessage(typeof(HttpController), Enums.DoshiiLogLevels.Debug, string.Format("Instantiating HttpController Class with; urlBase - '{0}', locationId - '{1}', vendor - '{2}', secretKey - '{3}'", urlBase, _controllersCollection.ConfigurationManager.GetLocationTokenFromPos(), _controllersCollection.ConfigurationManager.GetVendorFromPos(), _controllersCollection.ConfigurationManager.GetSecretKeyFromPos()));
            if (string.IsNullOrWhiteSpace(urlBase))
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), Enums.DoshiiLogLevels.Error, string.Format("Instantiating HttpController Class with a blank urlBase - '{0}'", urlBase));
                throw new ArgumentException("blank URL");
            
            }
            
            
            _doshiiUrlBase = urlBase;
        }

        #region Order methods

        internal virtual ObjectActionResult<List<Log>> GetUnlinkedOrderLog(string doshiiOrderId)
        {

            return MakeHttpRequestWithForResponseData(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.UnlinkedOrderLog, "get unliked order log", "", doshiiOrderId);
        }

        internal virtual ObjectActionResult<List<Log>> GetOrderLog(string orderId)
        {
            return MakeHttpRequestWithForResponseData(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.OrderLog, "get order log", "", orderId);
        }


        /// <summary>
        /// This method is used to retrieve the Order from Doshii matching the provided orderId (the pos identifier for the Order),
        /// </summary>
        /// <param name="orderId">
        /// The pos identifier for the Order
        /// </param>
        /// <returns>
        /// If an Order if found matching the orderId the Order is returned,
        /// If on Order matching the orderId is not found a new Order is returned. 
        /// </returns>
        internal virtual ObjectActionResult<Order> GetOrder(string orderId)
        {
            return MakeHttpRequestWithForResponseData<Order, JsonOrder>(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.Order, "get order", "", orderId);
        }

        /// <summary>
        /// This method is used to retrieve the Order from Doshii matching the provided doshiiOrderId (the doshii identifier for the Order),
        /// This method should only be used with trying to retreive orders from Doshii that are not currently linked to a pos Order, 
        /// If the orders are currently linked on the Pos <see cref="GetOrder"/> should be used. 
        /// </summary>
        /// <param name="orderId">
        /// The pos identifier for the Order
        /// </param>
        /// <returns>
        /// If an Order if found matching the orderId the Order is returned,
        /// If on Order matching the orderId is not found a new Order is returned. 
        /// </returns>
        internal virtual ObjectActionResult<Order> GetOrderFromDoshiiOrderId(string doshiiOrderId)
        {
            return MakeHttpRequestWithForResponseData<Order, JsonOrder>(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.UnlinkedOrders, "get unlinked order", "", doshiiOrderId);
        }

        /// <summary>
        /// Gets all the current active linked orders in Doshii.
        /// To get all Order including unlinked orders you must also call <see cref="GetUnlinkedOrders"/>
        /// </summary>
        /// <returns>
        /// A list of all currently active linked orders from Doshii
        /// If there are no current active linked orders an empty list is returned.  
        /// </returns>
        internal virtual ObjectActionResult<List<Order>> GetOrders()
        {
            var actionResult = MakeHttpRequestWithForResponseData<List<Order>, List<JsonOrder>>(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.Order, "get orders");

            var fullOrderList = new List<Models.Order>();
            foreach (var partOrder in actionResult.ReturnObject)
            {
                var newOrderResponse = GetOrder(partOrder.Id);
                if (newOrderResponse.ReturnObject != null)
                {
                    fullOrderList.Add(newOrderResponse.ReturnObject);
                }
            }
            actionResult.ReturnObject = fullOrderList;
            return actionResult;
        }

        /// <summary>
        /// Gets all the current active unlinked orders in Doshii.
        /// To get all Order including linked orders you must also call <see cref="GetOrders"/>
        /// </summary>
        /// <returns>
        /// A list of all currently active unlinked orders from Doshii
        /// If there are no current active unlinked orders an empty list is returned.  
        /// </returns>
        internal virtual ObjectActionResult<List<Order>> GetUnlinkedOrders()
        {
            var actionResult = MakeHttpRequestWithForResponseData<List<Order>, List<JsonOrder>>(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.UnlinkedOrders, "get unlinked orders");

            var fullOrderList = new List<Models.Order>();
            foreach (var partOrder in actionResult.ReturnObject)
            {
                var newOrderResponse = GetOrderFromDoshiiOrderId(partOrder.DoshiiId);
                if (newOrderResponse.ReturnObject != null)
                {
                    fullOrderList.Add(newOrderResponse.ReturnObject);
                }
            }
            actionResult.ReturnObject = fullOrderList;
            return actionResult;
            
        }

        /// <summary>
        /// completes the Put or Post request to update an Order with Doshii. 
        /// </summary>
        /// <param name="order">
        /// The Order to by updated on Doshii
        /// </param>
        /// <param name="method">
        /// The HTTP verb to be used (in the current version the only acceptable verb is PUT)
        /// </param>
        /// <returns>
        /// The Order returned from the request. 
        /// </returns>
        /// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
        internal virtual ObjectActionResult<Order> PutPostOrder(Order order, string method)
        {
            if (!method.Equals(WebRequestMethods.Http.Put))
            {
                throw new NotSupportedException("Method Not Supported");
            }

            try
            {
                ObjectActionResult<Order> response;
                var jsonOrderToPut = Mapper.Map<JsonOrderToPut>(order);
                if (String.IsNullOrEmpty(order.Id))
                {
                    response = MakeHttpRequestWithForResponseData<Order, JsonOrder>(60000, method, EndPointPurposes.UnlinkedOrders, "create order on doshii", jsonOrderToPut.ToJsonString());
                }
                else
                {
                    response = MakeHttpRequestWithForResponseData<Order, JsonOrder>(60000, method, EndPointPurposes.Order, "update order on doshii", jsonOrderToPut.ToJsonString(), order.Id);
                }
                UpdateOrderVersion<Order>(response.ReturnObject);
                UpdateOrderCheckin<Order>(response.ReturnObject);
                return response;
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method is specifically called to confirm an Order created on an orderAhead partner.
        /// </summary>
        /// <param name="order">
        /// The Order to be confirmed
        /// </param>
        /// <returns>
        /// The Order that was returned from the PUT request to Doshii. 
        /// </returns>
        internal virtual ObjectActionResult<Order> PutOrderCreatedResult(Models.Order order)
        {
            try
            {
                var jsonOrderToPut = Mapper.Map<JsonUnlinkedOrderToPut>(order);
                var response = MakeHttpRequestWithForResponseData<Order, JsonOrder>(60000, WebRequestMethods.Http.Put, EndPointPurposes.UnlinkedOrders, "confirm order on doshii", jsonOrderToPut.ToJsonString(), order.DoshiiId); 
                UpdateOrderVersion<Order>(response.ReturnObject);
                UpdateOrderCheckin<Order>(response.ReturnObject);
                return response;

            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This function takes the supplied <paramref name="responseMessage"/> received from the RESTful Doshii API and translates it
        /// into some sort of Order object. It utilises the mapping between a model object (<typeparamref name="T"/>) and its corresponding 
        /// JSON data transfer object (<typeparamref name="DTO"/>). The data transfer object type should be an extension of the 
        /// <see cref="JsonSerializationBase{TSelf}<TSelf>"/> class.
        /// </summary>
        /// <remarks>
        /// The purpose of this function is to provide a consistent manner of parsing the response to the <c>PUT /orders/:pos_id</c> call in the 
        /// API, regardless of the actual model object we are dealing with for the action taken.
        /// </remarks>
        /// <typeparam name="T">The type of model object to be returned by this call. This should be a member of the <c>DoshiiDotNetIntegration.Models</c>
        /// namespace that is mapped to the <typeparamref name="DTO"/> type via the <see cref="DoshiiDotNetIntegration.Helpers.AutoMapperConfigurator"/>
        /// helper class.</typeparam>
        /// <typeparam name="DTO">The corresponding data type object used by the communication with the API for the action.</typeparam>
        /// <param name="orderId">The POS identifier for the Order.</param>
        /// <param name="responseMessage">The current response message to be parsed.</param>
        /// <param name="jsonDto">When this function returns, this output parameter will be the data transfer object used in communication with the API.</param>
        /// <returns>The details of the Order in the Doshii API.</returns>
        internal T HandleOrderResponse<T, DTO>(string orderId, DoshiHttpResponseMessage responseMessage, out DTO jsonDto)
        {
            jsonDto = default(DTO); // null since its an object
            T returnObj = default(T); // null since its an object

            _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format(" The Response message has been returned to the put Order function"));

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format(" The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format(" The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format(" The Response Order data was not null"));
                        jsonDto = JsonConvert.DeserializeObject<DTO>(responseMessage.Data);
                        returnObj = Mapper.Map<T>(jsonDto);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format(" A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format(" A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format(" The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                throw new NullResponseDataReturnedException();
            }

            UpdateOrderVersion<T>(returnObj);
            UpdateOrderCheckin<T>(returnObj);

            return returnObj;
        }

        /// <summary>
        /// A call to this function updates the Order version in the POS. The generic nature of this function is due to the fact that
        /// we might be dealing with different actual model objects. This function can be used to update the POS version of the Order
        /// regardless of the actual type used.
        /// </summary>
        /// <remarks>
        /// NOTE: The SDK implementer must update this call for any new model types that make use of the Order version.
        /// </remarks>
        /// <typeparam name="T">The type of model object being updated. In this case, the type should be a derivative of an
        /// <see cref="DoshiiDotNetIntegration.Models.Order"/> or a class that contains a reference to an Order.</typeparam>
        /// <param name="orderDetails">The details of the Order.</param>
        internal virtual void UpdateOrderVersion<T>(T orderDetails)
        {
            if (orderDetails != null)
            {
                Models.Order order = null;
                if (orderDetails is Models.Order)
                    order = orderDetails as Models.Order;
                else if (orderDetails is TableOrder)
                    order = (orderDetails as TableOrder).Order;

                if (order != null && !String.IsNullOrEmpty(order.Id))
                    _controllersCollection.OrderingController.RecordOrderVersion(order.Id, order.Version);
            }
        }

        internal virtual void UpdateOrderCheckin<T>(T orderDetails)
        {
            if (orderDetails != null)
            {
                Models.Order order = null;
                if (orderDetails is Models.Order)
                    order = orderDetails as Models.Order;
                else if (orderDetails is TableOrder)
                    order = (orderDetails as TableOrder).Order;

                if (order != null && !String.IsNullOrEmpty(order.CheckinId))
                    _controllersCollection.OrderingController.RecordOrderCheckinId(order.Id, order.CheckinId);
            }
        }

        /// <summary>
        /// This method is used to confirm or reject or update an Order when the Order has an OrderId
        /// </summary>
        /// <param name="order">
        /// The Order to be updated
        /// </param>
        /// <returns>
        /// If the request is not successful a new Order will be returned - you can check the Order.Id in the returned Order to confirm it is a valid response. 
        /// </returns>
        internal virtual ObjectActionResult<Order> PutOrder(Models.Order order)
        {
            return PutPostOrder(order, WebRequestMethods.Http.Put);
        }

        
        /// <summary>
        /// Deletes a table allocation from doshii for the provided CheckinId. 
        /// </summary>
        /// <returns>
        /// true if successful
        /// false if failed
        /// </returns>
        ///<exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ActionResultBasic DeleteTableAllocation(string checkinId)
        {
            try
            {
                return MakeHttpRequest(60000, DeleteMethod, EndPointPurposes.DeleteAllocationFromCheckin,
                    "delete table allocation", "", checkinId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
        
        #endregion

        #region CreatedByApp App Methods

        internal virtual ObjectActionResult<List<App>> GetApps()
        {
            try
            {
                return MakeHttpRequestWithForResponseData<List<App>, List<JsonApp>>(60000,
                    WebRequestMethods.Http.Get, EndPointPurposes.App, "get apps");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
        
        #endregion

        #region transaction methods

        internal virtual ObjectActionResult<List<Log>> GetTransactionLog(string transactionId)
        {
            return MakeHttpRequestWithForResponseData(60000, WebRequestMethods.Http.Get,
                EndPointPurposes.TransactionLog, "get transaction log", "", transactionId);
        }

        /// <summary>
        /// This method is used to retrieve a list of transaction related to an Order with the doshiiOrderId
        /// This method will only retreive transactions for unlinked orders on Doshii - if the Order is linked to a pos Order there is no method to retreive the transaction in the OrderAhead implementation. 
        /// </summary>
        /// <param name="doshiiOrderId">
        /// the doshiiOrderId for the Order. 
        /// </param>
        /// <returns>
        /// A list of transactions associated with the Order,
        /// If there are no transaction associated with the Order an empty list is returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<List<Transaction>> GetTransactionsFromDoshiiOrderId(string doshiiOrderId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Transaction>, List<JsonTransaction>>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.TransactionFromDoshiiOrderId,
                        "get transactions for order", "", doshiiOrderId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method is used to retrieve a list of transaction related to an Order with the posOrderId
        /// </summary>
        /// <param name="posOrderId">
        /// the posOrderId for the Order. 
        /// </param>
        /// <returns>
        /// A list of transactions associated with the Order,
        /// If there are no transaction associated with the Order an empty list is returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<List<Transaction>> GetTransactionsFromPosOrderId(string posOrderId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Transaction>, List<JsonTransaction>>(60000,
                        WebRequestMethods.Http.Get, Enums.EndPointPurposes.TransactionFromPosOrderId,
                        "get transactions for order", "", posOrderId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method is used to get a transaction from Doshii with the matching transacitonId
        /// </summary>
        /// <param name="transactionId">
        /// The Id of the transaction to be retrieved.
        /// </param>
        /// <returns>
        /// The transaction with the given Id if it exists,
        /// A Blank transaction if it did not exist. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Transaction> GetTransaction(string transactionId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Transaction, JsonTransaction>(60000,
                        WebRequestMethods.Http.Get, Enums.EndPointPurposes.Transaction,
                        "get transaction", "", transactionId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// Gets all the current active transactions in Doshii. 
        /// </summary>
        /// <returns>
        /// an IEnumerable of transactions 
        /// it will be empty if no transactions exist. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<List<Transaction>> GetTransactions()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Transaction>, List<JsonTransaction>>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Transaction,
                        "get transactions");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// completes the Post request to create a transaction on Doshii. 
        /// </summary>
        /// <param name="transaction">the transaction to be created on Doshii</param>
        /// <returns>
        /// The transaction that was created on Doshii
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Transaction> PostTransaction(Transaction transaction)
        {
            try
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                return
                    MakeHttpRequestWithForResponseData<Transaction, JsonTransaction>(180000,
                        WebRequestMethods.Http.Post, EndPointPurposes.Transaction,
                        "post transactions", jsonTransaction.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// completes the Put request to update a transaction that already exists on Doshii. 
        /// </summary>
        /// <param name="transaction">
        /// The transaction to be updated. 
        /// </param>
        /// <returns>
        /// The updated transaction returned by Doshii.
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Transaction> PutTransaction(Transaction transaction)
        {
            try
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                return
                    MakeHttpRequestWithForResponseData<Transaction, JsonTransaction>(180000,
                        WebRequestMethods.Http.Put, EndPointPurposes.Transaction,
                        "put transactions", jsonTransaction.ToJsonString(), transaction.Id);
                
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
#endregion

        #region Member methods

        internal virtual ObjectActionResult<MemberOrg> GetMember(string memberId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<MemberOrg, JsonMember>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Members,
                        "get member", "", memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<List<MemberOrg>> GetMembers()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<MemberOrg>, List<JsonMember>>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Members,
                        "get members");
            }
            catch (Exception rex)
            {
                throw rex;
            }
    }


        internal virtual ObjectActionResult<MemberOrg> PutMember(MemberOrg member)
        {
            try
            {
                var jsonMember = Mapper.Map<JsonMemberToUpdate>(member);
                return
                    MakeHttpRequestWithForResponseData<MemberOrg, JsonMember>(60000,
                        WebRequestMethods.Http.Put, EndPointPurposes.Members,
                        "put members", jsonMember.ToJsonString(), member.Id);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<MemberOrg> PostMember(MemberOrg member)
        {
            try
            {
                var jsonMember = Mapper.Map<JsonMemberToUpdate>(member);
                return
                    MakeHttpRequestWithForResponseData<MemberOrg, JsonMember>(60000,
                        WebRequestMethods.Http.Post, EndPointPurposes.Members,
                        "post members", jsonMember.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic DeleteMember(string memberId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<MemberOrg, JsonMember>(60000,
                        HttpController.DeleteMethod, EndPointPurposes.Members,
                        "delete Member", "", memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<List<Reward>> GetRewardsForMember(string memberId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Reward>, List<JsonReward>>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.MemberGetRewards,
                        "get rewards for member", "", memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
}

        internal virtual ActionResultBasic RedeemRewardForMember(string memberId, string rewardId, Models.Order order)
        {
            
            try
            {
                var jsonOrderIdSimple = Mapper.Map<JsonOrderIdSimple>(order);
                return
                    MakeHttpRequest(180000,
                        WebRequestMethods.Http.Post, EndPointPurposes.MemberRewardsRedeem,
                        "redeem reward for member", jsonOrderIdSimple.ToJsonString(), memberId, rewardId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {

            try
            {
                return
                    MakeHttpRequest(180000,
                        WebRequestMethods.Http.Put, EndPointPurposes.MemberRewardsRedeemCancel,
                        "redeem reward for member cancel", "{ \"reason\": \"" + cancelReason + "\"}", memberId, rewardId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic RedeemRewardForMemberConfirm(string memberId, string rewardId)
        {

            try
            {
                return
                    MakeHttpRequest(180000,
                        WebRequestMethods.Http.Put, EndPointPurposes.MemberRewardsRedeemConfirm,
                        "redeem reward for member confirm", "", memberId, rewardId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic RedeemPointsForMember(PointsRedeem pr, MemberOrg member)
        {
            try
            {
                var jsonPointsRedeem = Mapper.Map<JsonPointsRedeem>(pr);
                return
                    MakeHttpRequest(180000,
                        WebRequestMethods.Http.Post, EndPointPurposes.MemberPointsRedeem,
                        "redeem points for member", jsonPointsRedeem.ToJsonString(), member.Id);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic RedeemPointsForMemberConfirm(string memberId)
        {
            try
            {
                return
                    MakeHttpRequest(180000,
                        WebRequestMethods.Http.Put, EndPointPurposes.MemberPointsRedeemConfirm,
                        "redeem points for member confirm", "", memberId);
        }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic RedeemPointsForMemberCancel(string memberId, string cancelReason)
        {
            try
            {
                return
                    MakeHttpRequest(180000,
                        WebRequestMethods.Http.Put, EndPointPurposes.MemberPointsRedeemCancel,
                        "redeem points for member cancel", "{ \"reason\": \"" + cancelReason + "\"}", memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
            }
        #endregion

        #region checkins / consumers
        /// <summary>
        /// This method is use to get a consumer from Doshii that corresponds with the CheckinId 
        /// </summary>
        /// <param name="checkinId">
        /// The CheckinId identifying the consumer. 
        /// </param>
        /// <returns>
        /// The consumer returned by doshii
        /// or a blank consumer if no consumer was returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Consumer> GetConsumerFromCheckinId(string checkinId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Consumer, JsonConsumer>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.ConsumerFromCheckinId,
                        "get consumer from CheckinId", "", checkinId);
            }
            catch (Exception rex)
            {
                throw rex;
            }


        }

        internal virtual ObjectActionResult<Checkin> PostCheckin(Checkin checkin)
        {
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                return
                    MakeHttpRequestWithForResponseData<Checkin, JsonCheckin>(60000,
                        WebRequestMethods.Http.Post, EndPointPurposes.Checkins,
                        "post checkin", jsonCheckin.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Checkin> PutCheckin(Checkin checkin)
        {
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                return
                    MakeHttpRequestWithForResponseData<Checkin, JsonCheckin>(60000,
                        WebRequestMethods.Http.Put, EndPointPurposes.Checkins,
                        "put checkin", jsonCheckin.ToJsonString(), checkin.Id);
            }
            catch (Exception rex)
            {
                throw new CheckinUpdateException("Exception updating checkin", rex);
            }
        }

        internal virtual ActionResultBasic DeleteCheckin(string checkinId)
        {
            try
            {
                return
                    MakeHttpRequest(60000, DeleteMethod, EndPointPurposes.Checkins,
                        "delete checkin", "", checkinId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Checkin> GetCheckin(string checkinId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Checkin, JsonCheckin>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Checkins,
                        "Get checkin", "", checkinId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Checkin> GetNewCheckin()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Checkin, JsonCheckin>(60000,
                        WebRequestMethods.Http.Post, EndPointPurposes.Checkins,
                        "Get checkin");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<List<Checkin>> GetCheckins()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Checkin>, List<JsonCheckin>>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Checkins,
                        "Get checkins");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

#endregion

        #region Menu

        internal virtual ObjectActionResult<Menu> GetMenu()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Menu, JsonMenu>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Menu,
                        "get menu");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
        
        
        /// <summary>
        /// Adds a menu to Doshii, this will overwrtie the current menu stored on Doshii 
        /// </summary>
        /// <param name="menu">
        /// The menu to be added to Doshii
        /// </param>
        /// <returns>
        /// The menu that was added to doshii. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Menu> PostMenu(Menu menu)
        {
            try
            {
                var jsonMenu = Mapper.Map<JsonMenu>(menu);
                return
                    MakeHttpRequestWithForResponseData<Menu, JsonMenu>(60000,
                        WebRequestMethods.Http.Post, EndPointPurposes.Menu,
                        "post menu", jsonMenu.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// adds or updates a surcount on the pos menu in doshii. 
        /// </summary>
        /// <param name="surcount">
        /// the surcount to be added or updated on Doshii
        /// </param>
        /// <returns>
        /// The surcount that was updated on Doshii
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Surcount> PutSurcount(Surcount surcount)
        {
            try
            {
                var jsonSurcount = Mapper.Map<JsonMenuSurcount>(surcount);
                return
                    MakeHttpRequestWithForResponseData<Surcount, JsonMenuSurcount>(60000,
                        WebRequestMethods.Http.Put, EndPointPurposes.Surcounts,
                        "put surcount", jsonSurcount.ToJsonString(), surcount.Id);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// Deletes a surcount from the pos menu on doshii
        /// </summary>
        /// <param name="posId">
        /// The pos Id related to the surcount on Doshii
        /// </param>
        /// <returns></returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ActionResultBasic DeleteSurcount(string posId)
        {
            try
            {
                return
                    MakeHttpRequest(60000,
                        DeleteMethod, EndPointPurposes.Surcounts,
                        "delete surcount", "", posId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// adds or updates a product on the pos menu on doshii
        /// </summary>
        /// <param name="product">
        /// The product to be added or updated
        /// </param>
        /// <returns>
        /// The product that was added or updated. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ObjectActionResult<Product> PutProduct(Product product)
        {
            try
            {
                var jsonProduct = Mapper.Map<JsonMenuProduct>(product);
                return
                    MakeHttpRequestWithForResponseData<Product, JsonMenuProduct>(60000,
                        WebRequestMethods.Http.Put, EndPointPurposes.Products,
                        "put product", jsonProduct.ToJsonString(), product.PosId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// deletes the product from the pos menu on doshii
        /// </summary>
        /// <param name="posId">
        /// The posId of the product to be deleted. 
        /// </param>
        /// <returns>
        /// true if the product was deleted,
        /// false if the product was not deleted. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual ActionResultBasic DeleteProduct(string posId)
        {
            try
            {
                return
                    MakeHttpRequest(60000,
                        DeleteMethod, EndPointPurposes.Products,
                        "delete product", "", posId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
#endregion

        #region Location
        /// <summary>
        /// This method is used to retrieve the location information for the connected pos from doshii,
        /// </summary>
        /// <returns>
        /// The location information for the connected venue in doshii
        /// </returns>
        internal virtual ObjectActionResult<Location> GetLocation()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Location, JsonLocation>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Location,
                        "get location");
                
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Location> PostLocation(Location location)
        {
            try
            {
                var locationToPost = Mapper.Map<JsonLocation>(location);
                return
                    MakeHttpRequestWithForResponseData<Location, JsonLocation>(60000,
                            WebRequestMethods.Http.Post, EndPointPurposes.Locations,
                            "post location", locationToPost.ToJsonString(), "","",true);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<List<Location>> GetLocations()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Location>, List<JsonLocation>>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.Locations,
                            "get locations", "", "", "", true);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Location> GetLocation(string hashedLocationId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Location, JsonLocation>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Locations,
                        "get location", "", hashedLocationId, "", true);
                
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Organisation> PostOrginisation(Organisation organisation)
        {
            try
            {
                var orginisationToPost = Mapper.Map<JsonOrganisation>(organisation);
                return
                    MakeHttpRequestWithForResponseData<Organisation, JsonOrganisation>(60000,
                            WebRequestMethods.Http.Post, EndPointPurposes.Orginisation,
                            "post organisation", orginisationToPost.ToJsonString(), "", "", true);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

#endregion

        #region Tables

        internal virtual ObjectActionResult<Table> PostTable(Table table)
        {
            try
            {
                var jsonTable = Mapper.Map<JsonTable>(table);
                return
                    MakeHttpRequestWithForResponseData<Table, JsonTable>(60000,
                            WebRequestMethods.Http.Post, EndPointPurposes.Tables,
                            "post table", jsonTable.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Table> PutTable(Table table, string oldTableName)
        {
            try
            {
                var jsonTable = Mapper.Map<JsonTable>(table);
                return
                    MakeHttpRequestWithForResponseData<Table, JsonTable>(60000,
                            WebRequestMethods.Http.Put, EndPointPurposes.Tables,
                            "put table", jsonTable.ToJsonString(), oldTableName);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<List<Table>> PutTables(List<Table> tables)
        {
            try
            {
                List<JsonTable> jsonTableList = new List<JsonTable>();
                foreach (var t in tables)
                {
                    jsonTableList.Add(Mapper.Map<JsonTable>(t));
                }
                return
                    MakeHttpRequestWithForResponseData<List<Table>, List<JsonTable>>(60000,
                            WebRequestMethods.Http.Put, EndPointPurposes.Tables,
                            "put tables", JsonConvert.SerializeObject(jsonTableList));
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic DeleteTable(string tableName)
        {
            try
            {
                return
                    MakeHttpRequest(60000,
                            DeleteMethod, EndPointPurposes.Tables,
                            "delete table", "", tableName);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Table> GetTable(string tableName)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Table, JsonTable>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.Tables,
                            "get table", "", tableName);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<List<Table>>  GetTables()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Table>, List<JsonTable>>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.Tables,
                            "get tables");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        #endregion

        #region BookingsWithDateFilter

        internal ObjectActionResult<Booking> PutBooking(Booking booking)
        {
            try
            {
                var jsonBooking = Mapper.Map<JsonBooking>(booking);
                return
                    MakeHttpRequestWithForResponseData<Booking, JsonBooking>(60000,
                            WebRequestMethods.Http.Put, EndPointPurposes.Booking,
                            "put booking", jsonBooking.ToJsonString(), booking.Id);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal ObjectActionResult<Booking> PostBooking(Booking booking)
        {
            try
            {
                var jsonBooking = Mapper.Map<JsonBooking>(booking);
                return
                    MakeHttpRequestWithForResponseData<Booking, JsonBooking>(60000,
                            WebRequestMethods.Http.Post, EndPointPurposes.Booking,
                            "post booking", jsonBooking.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal ActionResultBasic DeleteBooking(string bookingId)
        {
            try
            {
                return
                    MakeHttpRequest(60000,
                            DeleteMethod, EndPointPurposes.Booking,
                            "delete booking", "", bookingId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
        
        internal ObjectActionResult<Checkin> SeatBooking(string bookingId, Checkin checkin)
        {
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                return
                    MakeHttpRequestWithForResponseData<Checkin, JsonCheckin>(60000,
                            WebRequestMethods.Http.Put, EndPointPurposes.BookingsCheckin,
                            "seat booking", jsonCheckin.ToJsonString(), bookingId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal ObjectActionResult<Checkin> SeatBookingWithoutCheckin(string bookingId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Checkin, JsonCheckin>(60000,
                            WebRequestMethods.Http.Post, EndPointPurposes.BookingsCheckin,
                            "seat booking without checkin", "", bookingId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Booking> GetBooking(String bookingId)
        {
            try
            {
                var booking =
                    MakeHttpRequestWithForResponseData<Booking, JsonBooking>(60000,
                        WebRequestMethods.Http.Get, EndPointPurposes.Booking,
                        "get booking", "", bookingId);
                if (booking.Success && booking.ReturnObject != null)
                {

                    var b = booking.ReturnObject;
                    if (b.Checkin != null && !string.IsNullOrWhiteSpace(b.Checkin.Id))
                    {
                        b.CheckinId = b.Checkin.Id;

                        var checkinResult = GetCheckin(b.CheckinId);
                        if (checkinResult != null && checkinResult.ReturnObject != null && checkinResult.Success)
                        {
                            b.Checkin = checkinResult.ReturnObject;
                        }

                    }


                }
                return booking;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal virtual ObjectActionResult<List<Booking>> GetBookings(DateTime from, DateTime to, object query = null)
        {
            try
            {
                var bookings=
                    MakeHttpRequestWithForResponseData<List<Booking>, List<JsonBooking>>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.BookingsWithDateFilter,
                            "get bookings", "", from.ToEpochSeconds().ToString(), to.ToEpochSeconds().ToString(),query:query);

                if (bookings.ReturnObject != null)
                {
                    bookings.ReturnObject.ForEach(b =>
                    {
                        if (b.Checkin != null )
                        {
                            b.CheckinId = b.Checkin.Id;

                            var checkinResult = GetCheckin(b.CheckinId);
                            if (checkinResult != null && checkinResult.ReturnObject != null && checkinResult.Success)
                            {
                                b.Checkin = checkinResult.ReturnObject;
                            }
                        }
                    } );
                }
                return bookings;
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
        #endregion

        #region RejectionCodes

        internal virtual ObjectActionResult<List<RejectionCode>> GetRejectionCodes()
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<RejectionCode>, List<JsonRejectionCode>>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.RejectionCodes,
                            "get rejectionCodes");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<RejectionCode> GetRejectionCode(string code)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<RejectionCode, JsonRejectionCode>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.RejectionCodes,
                            "get rejectionCode", "", code);
            }
            catch (Exception rex)
            {
                throw rex;
            }

        }
        
        #endregion

        #region Employee

        internal virtual ObjectActionResult<List<Employee>> GetEmployees()
        {
            var retreivedEmployeeList = new List<Models.Employee>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                return
                    MakeHttpRequestWithForResponseData<List<Employee>, List<JsonEmployee>>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.Employee,
                            "get employees");
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Employee> GetEmployee(string doshiiId)
        {
            try
            {
                return
                    MakeHttpRequestWithForResponseData<Employee, JsonEmployee>(60000,
                            WebRequestMethods.Http.Get, EndPointPurposes.Employee,
                            "get employee", "", doshiiId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Employee> PostEmployee(Employee employee)
        {
            try
            {
                var employeeToPost = Mapper.Map<JsonEmployee>(employee);
                return
                    MakeHttpRequestWithForResponseData<Employee, JsonEmployee>(60000,
                            WebRequestMethods.Http.Post, EndPointPurposes.Employee,
                            "post employee", employeeToPost.ToJsonString());
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ObjectActionResult<Employee> PutEmployee(Employee employee)
        {
            try
            {
                var employeeToPut = Mapper.Map<JsonEmployee>(employee);
                return
                    MakeHttpRequestWithForResponseData<Employee, JsonEmployee>(60000,
                            WebRequestMethods.Http.Put, EndPointPurposes.Employee,
                            "put employee", employeeToPut.ToJsonString(), employee.Id);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        internal virtual ActionResultBasic DeleteEmployee(string employeeId)
        {
            try
            {
                return
                    MakeHttpRequest(60000,
                            DeleteMethod, EndPointPurposes.Employee,
                            "delete employee", "", employeeId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        #endregion



        #region comms helper methods

        /// <summary>
        /// Generates a URL based on the base URL and the purpose of the message that is being sent. 
        /// </summary>
        /// <param name="purpose">
        /// An <see cref="EndPointPurposes"/> the represents the purpose of the request
        /// </param>
        /// <param name="identification">
        /// An optional identifier used in the request 
        /// eg, the orderId for a get Order request
        /// </param>
        /// <returns>
        /// The Url required to make the desiered request. 
        /// </returns>
        internal virtual string GenerateUrl(EndPointPurposes purpose, string identification = "", string secondIdentification = "", object query=null)
        {
            StringBuilder newUrlbuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(identification))
            {
                identification = System.Uri.EscapeDataString(identification);   
            }
            if (!string.IsNullOrEmpty(secondIdentification))
            {
                secondIdentification = System.Uri.EscapeDataString(secondIdentification);
            }
            if (string.IsNullOrWhiteSpace(_doshiiUrlBase))
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error, " The HttpController class was not initialized correctly, the base URl is null or white space");
                return newUrlbuilder.ToString();
            }
            newUrlbuilder.AppendFormat("{0}", _doshiiUrlBase);

            var queryString = new StringBuilder();
            if (query != null)
            {
                ExtractData(query)
                    .Where(kvp => kvp.Value != null)
                    .ToList()
                    .ForEach(kvp => queryString.AppendFormat("{0}={1}&", kvp.Key,
                        Uri.EscapeDataString(kvp.Value.ToString())));

                if (queryString.Length > 1)
                {
                    queryString.Remove(queryString.Length - 1, 1);
                }
            }

            switch (purpose)
            {
				case EndPointPurposes.Order:
                    newUrlbuilder.Append("/orders");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.DeleteAllocationFromCheckin:
                    newUrlbuilder.AppendFormat("/tables?checkin={0}", identification);
                    break;
                case EndPointPurposes.Transaction:
                    newUrlbuilder.Append("/transactions");
					if (!String.IsNullOrWhiteSpace(identification))
					{
						newUrlbuilder.AppendFormat("/{0}", identification);
					}
                    break;
                case EndPointPurposes.TransactionFromDoshiiOrderId:
                    newUrlbuilder.AppendFormat("/unlinked_orders/{0}/transactions", identification);
                    break;
                case EndPointPurposes.TransactionFromPosOrderId:
                    newUrlbuilder.AppendFormat("/orders/{0}/transactions", identification);
                    break;
                case EndPointPurposes.UnlinkedOrders:
                    newUrlbuilder.Append("/unlinked_orders");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.ConsumerFromCheckinId:
                    newUrlbuilder.AppendFormat("/checkins/{0}/consumer", identification);
                    break;
                case EndPointPurposes.Menu:
                    newUrlbuilder.AppendFormat("/menu");
                    break;
                case EndPointPurposes.Products:
                    newUrlbuilder.Append("/menu/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Surcounts:
                    newUrlbuilder.Append("/menu/surcounts");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Location:
                    newUrlbuilder.Append("/location");
                    break;
                case EndPointPurposes.Members:
                    newUrlbuilder.Append("/members");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.MemberRewards:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards", identification);
                    break;
                case EndPointPurposes.MemberRewardsRedeem:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards/{1}/redeem", identification, secondIdentification);
                    break;
                case EndPointPurposes.MemberRewardsRedeemConfirm:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards/{1}/confirm", identification, secondIdentification);
                    break;
                case EndPointPurposes.MemberRewardsRedeemCancel:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards/{1}/cancel", identification, secondIdentification);
                    break;
                case EndPointPurposes.MemberPointsRedeem:
                    newUrlbuilder.AppendFormat("/members/{0}/points/redeem", identification);
                    break;
                case EndPointPurposes.MemberPointsRedeemConfirm:
                    newUrlbuilder.AppendFormat("/members/{0}/points/confirm", identification);
                    break;
                case EndPointPurposes.MemberPointsRedeemCancel:
                    newUrlbuilder.AppendFormat("/members/{0}/points/cancel", identification);
                    break;
                case EndPointPurposes.Checkins:
                    newUrlbuilder.Append("/checkins");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Tables:
                    newUrlbuilder.Append("/tables");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Booking:
                    newUrlbuilder.AppendFormat("/bookings");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.BookingsWithDateFilter:
                    newUrlbuilder.Append("/bookings");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("?from={0}&to={1}&{2}", identification, secondIdentification, queryString);
                    }
                    break;
                case EndPointPurposes.BookingsCheckin:
                    newUrlbuilder.AppendFormat("/bookings/{0}/checkin", identification);
                    break;
                case EndPointPurposes.App:
                    newUrlbuilder.AppendFormat("/apps");
                    break;
                case EndPointPurposes.RejectionCodes:
                    newUrlbuilder.Append("/rejection_codes");
					if (!String.IsNullOrWhiteSpace(identification))
					{
						newUrlbuilder.AppendFormat("/{0}", identification);
					}
                    break;
                case EndPointPurposes.Employee:
                    newUrlbuilder.Append("/employees");
                    if (!String.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Orginisation:
                    newUrlbuilder.Append("/organisations");
                    break;
                case EndPointPurposes.OrderLog:
                    newUrlbuilder.AppendFormat("/orders/{0}/logs", identification);
                    break;
                case EndPointPurposes.UnlinkedOrderLog:
                    newUrlbuilder.AppendFormat("/unlinked_orders/{0}/logs", identification);
                    break;
                case EndPointPurposes.Locations:
                    newUrlbuilder.Append("/locations");
                    break;
                case EndPointPurposes.MemberGetRewards:
                    newUrlbuilder.Append("/members");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}/rewards", identification);
                    }
                    break;
                case EndPointPurposes.TransactionLog:
                    newUrlbuilder.AppendFormat("/transactions/{0}/logs", identification);
                    break;
                default:
                    throw new NotSupportedException(purpose.ToString());
            }

            return newUrlbuilder.ToString();
        }

        private static IDictionary<string, object> ExtractData(object dataAsAnonymousType)
        {
            var data = new Dictionary<string, object>();

            var properties = TypeDescriptor.GetProperties(dataAsAnonymousType);

            foreach (PropertyDescriptor property in properties)
            {
                data.Add(property.Name, property.GetValue(dataAsAnonymousType));
            }

            return data;
        }

        private string GetHttpMethodFromMethodString(string methodString)
        {
            if (methodString.Equals(WebRequestMethods.Http.Get) ||
                methodString.Equals(WebRequestMethods.Http.Put) ||
                methodString.Equals(WebRequestMethods.Http.Post) ||
                methodString.Equals(HttpController.DeleteMethod))
            {
                return methodString;
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error, string.Format("MakeRequest was called with a non supported HTTP request method type - '{0}", methodString));
                throw new NotSupportedException("Invalid HTTP request Method Type");
            }
        }

        internal virtual ActionResultBasic MakeHttpRequest(int timeoutMilliseconds, string httpVerb, EndPointPurposes endPointPurpose, string processName, string requestData = "", string firstIdentifier = "", string secondIdentifier = "")
        {
            var actionResult = new ActionResultBasic();
            DoshiHttpResponseMessage responseMessage;
            string requestMethod = GetHttpMethodFromMethodString(httpVerb);
            string urlForRequest = GenerateUrl(endPointPurpose, firstIdentifier, secondIdentifier);
            try
            {
                responseMessage = MakeRequest(urlForRequest, requestMethod, timeoutMilliseconds, requestData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            if (responseMessage != null)
            {
                actionResult.responseStatusCode = responseMessage.Status;
                if (responseMessage.Status == HttpStatusCode.OK || responseMessage.Status == HttpStatusCode.Created)
                {
                    actionResult.Success = true;
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController),
                            DoshiiLogLevels.Warning,
                            DoshiiStrings.GetSuccessfulHttpResponseMessagesWithData(requestMethod,
                                urlForRequest, responseMessage.Data));
                }
                else
                {
                    actionResult.Success = false;
                    actionResult.FailReason = responseMessage.ErrorMessage;
                }
            }
            else
            {
                actionResult.Success = false;
                actionResult.FailReason = DoshiiStrings.GetUnknownErrorString(string.Format(processName));
            }

            return actionResult;
        }

        internal virtual ObjectActionResult<TReturnType> MakeHttpRequestWithForResponseData<TReturnType, TJsonReturnType>(int timeoutMilliseconds, string httpVerb, EndPointPurposes endPointPurpose, string processName, string requestData = "", string firstIdentifier = "", string secondIdentifier = "", bool useSecretKeyAsBearerAuth = false, object query = null)
            where TReturnType : class, new()
        {
            var actionResult = new ObjectActionResult<TReturnType>();
            DoshiHttpResponseMessage responseMessage;
            string requestMethod = GetHttpMethodFromMethodString(httpVerb);
            string urlForRequest = GenerateUrl(endPointPurpose, firstIdentifier, secondIdentifier, query);

            try
            {
                responseMessage = MakeRequest(urlForRequest, requestMethod, timeoutMilliseconds, requestData, useSecretKeyAsBearerAuth);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (responseMessage != null)
            {
                actionResult.responseStatusCode = responseMessage.Status;
                if (responseMessage.Status == HttpStatusCode.OK || responseMessage.Status == HttpStatusCode.Created)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<TJsonReturnType>(responseMessage.Data);
                        actionResult.ReturnObject =
                            AutoMapperGenericsHelper<TJsonReturnType, TReturnType>.ConvertToDBEntity(jsonList);
                        
                    }
                    actionResult.Success = true;
                }
                else
                {
                    actionResult.Success = false;
                    actionResult.FailReason = responseMessage.ErrorMessage;
                }
            }
            else
            {
                actionResult.Success = false;
                actionResult.FailReason = DoshiiStrings.GetUnknownErrorString(string.Format(processName));
            }

            return actionResult;
        }

        internal virtual ObjectActionResult<List<Log>> MakeHttpRequestWithForResponseData(int timeoutMilliseconds, string httpVerb, EndPointPurposes endPointPurpose, string processName, string requestData = "", string firstIdentifier = "", string secondIdentifier = "", bool useSecretKeyAsBearerAuth = false, object query = null)
        {
            var actionResult = new ObjectActionResult<List<Log>>();
            DoshiHttpResponseMessage responseMessage;
            string requestMethod = GetHttpMethodFromMethodString(httpVerb);
            string urlForRequest = GenerateUrl(endPointPurpose, firstIdentifier, secondIdentifier, query);

            try
            {
                responseMessage = MakeRequest(urlForRequest, requestMethod, timeoutMilliseconds, requestData, useSecretKeyAsBearerAuth);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (responseMessage != null)
            {
                actionResult.responseStatusCode = responseMessage.Status;
                if (responseMessage.Status == HttpStatusCode.OK || responseMessage.Status == HttpStatusCode.Created)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonLog>>(responseMessage.Data);
                        actionResult.ReturnObject = Mapper.Map<List<Log>>(jsonList);
                            //AutoMapperGenericsHelper<JsonLog, Log>.ConvertToDBEntity(jsonList);
                        
                        /*string contentCorrected = responseMessage.Data.Replace(".", "_");
                        var json = JsonConvert.DeserializeObject<dynamic>(contentCorrected);
                        var jo = json.Children<JObject>();
                        var logList = new List<Log>();
                        foreach (var data in jo)
                        {
                            if (data.action.Type == JTokenType.String)
                            {
                                var newLog = new Log();
                                newLog.Action = (string)data.action.Value;
                                newLog.AppId = (string) data.appId.Value;
                                newLog.AppName = (string) data.appName.Value;
                                logList.Add(newLog);
                            }
                        }
                        actionResult.ReturnObject = logList;*/
                    }
                    actionResult.Success = true;
                }
                else
                {
                    actionResult.Success = false;
                    actionResult.FailReason = responseMessage.ErrorMessage;
                }
            }
            else
            {
                actionResult.Success = false;
                actionResult.FailReason = DoshiiStrings.GetUnknownErrorString(string.Format(processName));
            }

            return actionResult;
        }

        /// <summary>
        /// makes a request HTTP to doshii based on the parameters provided. 
        /// </summary>
        /// <param name="url">
        /// The URL for the request - should be generated by <see cref="GenerateUrl"/>
        /// </param>
        /// <param name="method">
        /// The HTTP verb used for the request
        /// the following four verbs can be used
        /// <Item>GET</Item>
        /// <Item>PUT</Item>
        /// <Item>POST</Item>
        /// <Item>DELETE</Item>
        /// </param>
        /// <param name="data">
        /// The data that will be sent with the request. 
        /// eg, a JSON representation of the Order that should be send to Doshii with a PUT Order request. 
        /// </param>
        /// <returns></returns>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown when any of the following responses are received.
        /// <item> HttpStatusCode.BadRequest </item> 
        /// <item> HttpStatusCode.Unauthorized </item> 
        /// <item> HttpStatusCode.Forbidden </item>
        /// <item> HttpStatusCode.InternalServerError </item>
        /// <item> HttpStatusCode.NotFound </item> 
        /// <item> HttpStatusCode.Conflict </item>
        /// This must be handled where a conflict needs special treatment - this is especially important when orders are being updated by both the pos and the partner. 
        /// </exception>
        private DoshiHttpResponseMessage MakeRequest(string url, string method, int timeoutMilliseconds, string data = "", bool createOrginisation = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error, string.Format("MakeRequest was called without a URL"));
                throw new NotSupportedException("request with blank URL");
            }

            if (string.IsNullOrWhiteSpace(method))
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error, string.Format("MakeRequest was called without a HTTP method"));
                throw new NotSupportedException("request with blank HTTP method");
            }

            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Timeout = timeoutMilliseconds;
            if (createOrginisation)
            {
                request.Headers.Add("authorization", AuthHelper.CreateTokenForOrginisationCreate(_controllersCollection.ConfigurationManager.GetSecretKeyFromPos()));
            }
            else
            {
                request.Headers.Add("authorization", AuthHelper.CreateToken(_controllersCollection.ConfigurationManager.GetLocationTokenFromPos(), _controllersCollection.ConfigurationManager.GetSecretKeyFromPos()));
            }
            request.Headers.Add("vendor", _controllersCollection.ConfigurationManager.GetVendorFromPos());
            request.ContentType = "application/json";


            request.Method = GetHttpMethodFromMethodString(method);
            if (!string.IsNullOrWhiteSpace(data))
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }

            DoshiHttpResponseMessage responceMessage = new DoshiHttpResponseMessage();
            try
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format(" generating {0} request to endpoint {1}, with data {2}", method, url, data));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                responceMessage.Status = response.StatusCode;
                responceMessage.StatusDescription = responceMessage.StatusDescription;

                StreamReader sr = new StreamReader(response.GetResponseStream());
                responceMessage.Data = sr.ReadToEnd();

                sr.Close();
                response.Close();

                if (responceMessage.Status == HttpStatusCode.OK || responceMessage.Status == HttpStatusCode.Created)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController),
                        DoshiiLogLevels.Info,
                        DoshiiStrings.GetSuccessfulHttpResponseMessagesWithData(method,
                            url, responceMessage.Data));
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning,
                        DoshiiStrings.GetUnsucessfulHttpResponseMessage(method,
                            url, responceMessage.Data));
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (WebResponse response = wex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse) response;
                        //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                        string errorResponce;
                        using (Stream responceErrorData = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(responceErrorData))
                            {
                                errorResponce = reader.ReadToEnd();
                                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error,
                                    String.Format("Error code: {0}, ErrorResponse {1}", httpResponse.StatusCode,
                                        errorResponce));
                            }
                        }
                        
                        responceMessage.Status = httpResponse.StatusCode;
                        responceMessage.StatusDescription = httpResponse.StatusDescription;
                        if ((int)httpResponse.StatusCode >= 500 && (int)httpResponse.StatusCode < 600)
                        {
                            throw new WebException(wex.Message, wex.Status);
                        }
                        var theErrorMessage = DoshiiHttpErrorMessage.deseralizeFromJson(errorResponce);
                        responceMessage.ErrorMessage = theErrorMessage.Message;
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning,
                            DoshiiStrings.GetUnsucessfulHttpResponseMessage(method,
                                url, errorResponce + " " + wex));
                    }
                }
                else
                {
                    responceMessage.StatusDescription =
                        "There was no response in the web exception while making a request.";
                    responceMessage.ErrorMessage = wex.Message;

                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning,
                            DoshiiStrings.GetNullHttpResponseMessage(method,
                                url, "There was no response in the web exception while making a request." + " " + wex));
                }
                
            }
            catch (Exception ex)
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), Enums.DoshiiLogLevels.Error, string.Format(" As exception was thrown while attempting a {0} request to endpoint {1}, with data {2} and status {3} : {4}", method, url, data, responceMessage.Status.ToString(), ex));
                responceMessage.StatusDescription =
                        "There was no response in the web exception while making a request.";
                responceMessage.ErrorMessage = ex.ToString();

                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error,
                           DoshiiStrings.GetNullHttpResponseMessage(method,
                               url, "There was no response in the web exception while making a request." + " " + ex));
                throw ex;
            }
            return responceMessage;
        }

        

#endregion

    }
}
