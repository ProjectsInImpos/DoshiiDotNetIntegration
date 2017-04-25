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
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using DoshiiDotNetIntegration.Controllers;
using JWT;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;

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
		/// The <see cref="LoggingController"/> that is responsible for logging doshii messages, <see cref="_controllersCollection.LoggingController"/>
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

        #region order methods

        /// <summary>
        /// This method is used to retrieve the order from Doshii matching the provided orderId (the pos identifier for the order),
        /// </summary>
        /// <param name="orderId">
        /// The pos identifier for the order
        /// </param>
        /// <returns>
        /// If an order if found matching the orderId the order is returned,
        /// If on order matching the orderId is not found a new order is returned. 
        /// </returns>
        internal virtual Models.Order GetOrder(string orderId)
        {
            var retreivedOrder = new Models.Order();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, orderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
						var jsonOrder = JsonOrder.deseralizeFromJson(responseMessage.Data);
						retreivedOrder = Mapper.Map<Models.Order>(jsonOrder);
                    }
                    else
                    {
						_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
					_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
            }

            return retreivedOrder;
        }

        /// <summary>
        /// This method is used to retrieve the order from Doshii matching the provided doshiiOrderId (the doshii identifier for the order),
        /// This method should only be used with trying to retreive orders from Doshii that are not currently linked to a pos order, 
        /// If the orders are currently linked on the Pos <see cref="GetOrder"/> should be used. 
        /// </summary>
        /// <param name="orderId">
        /// The pos identifier for the order
        /// </param>
        /// <returns>
        /// If an order if found matching the orderId the order is returned,
        /// If on order matching the orderId is not found a new order is returned. 
        /// </returns>
        internal virtual Models.Order GetOrderFromDoshiiOrderId(string doshiiOrderId)
        {
            var retreivedOrder = new Models.Order();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonOrder = JsonOrder.deseralizeFromJson(responseMessage.Data);
                        try
                        {
                            retreivedOrder = Mapper.Map<Models.Order>(jsonOrder);
                        }
                        catch (Exception ex)
                        {
                            _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error,
                                string.Format(
                                    "Doshii: An order received from Doshii could not be processed, A Price value in the order could not be converted into a decimal, the order will be rejected by the SDK: ",
                                    jsonOrder),ex);
                            //reject the order. 
                            var orderToReject = Mapper.Map<Models.Order>(jsonOrder);
                            _controllersCollection.OrderingController.RejectOrderAheadCreation(orderToReject);
                            retreivedOrder = null;
                        }
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning,
                            string.Format(
                                "Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response",
                                GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId)));
            }

            return retreivedOrder;
        }

        /// <summary>
        /// Gets all the current active linked orders in Doshii.
        /// To get all order including unlinked orders you must also call <see cref="GetUnlinkedOrders"/>
        /// </summary>
        /// <returns>
        /// A list of all currently active linked orders from Doshii
        /// If there are no current active linked orders an empty list is returned.  
        /// </returns>
        internal virtual IEnumerable<Models.Order> GetOrders()
        {
            var retreivedOrderList = new List<Models.Order>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonOrder>>(responseMessage.Data);
                        retreivedOrderList = Mapper.Map<List<Models.Order>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order)));
            }

            List<Models.Order> fullOrderList = new List<Models.Order>();
            foreach (var partOrder in retreivedOrderList)
            {
                Models.Order newOrder = GetOrder(partOrder.Id);
                fullOrderList.Add(newOrder);
            }
            return (IEnumerable<Models.Order>)fullOrderList;
        }

        /// <summary>
        /// Gets all the current active unlinked orders in Doshii.
        /// To get all order including linked orders you must also call <see cref="GetOrders"/>
        /// </summary>
        /// <returns>
        /// A list of all currently active unlinked orders from Doshii
        /// If there are no current active unlinked orders an empty list is returned.  
        /// </returns>
        internal virtual IEnumerable<Models.Order> GetUnlinkedOrders()
        {
            var retreivedOrderList = new List<Models.Order>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonOrder>>(responseMessage.Data);
                        retreivedOrderList = Mapper.Map<List<Models.Order>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order)));
            }

            var fullOrderList = new List<Models.Order>();
            foreach (var partOrder in retreivedOrderList)
            {
                Models.Order newOrder = GetOrderFromDoshiiOrderId(partOrder.DoshiiId);
                if (newOrder != null)
                {
                    fullOrderList.Add(newOrder);
                }
            }
            return (IEnumerable<Models.Order>)fullOrderList;
        }

        /// <summary>
        /// completes the Put or Post request to update an order with Doshii. 
        /// </summary>
        /// <param name="order">
        /// The order to by updated on Doshii
        /// </param>
        /// <param name="method">
        /// The HTTP verb to be used (in the current version the only acceptable verb is PUT)
        /// </param>
        /// <returns>
        /// The order returned from the request. 
        /// </returns>
        /// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
        internal virtual Models.Order PutPostOrder(Models.Order order, string method)
        {
            if (!method.Equals(WebRequestMethods.Http.Put))
            {
                throw new NotSupportedException("Method Not Supported");
            }

            var returnOrder = new Models.Order();
            DoshiHttpResponseMessage responseMessage;

            try
            {
                var jsonOrderToPut = Mapper.Map<JsonOrderToPut>(order);
                if (String.IsNullOrEmpty(order.Id))
                {
                    responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders, order.DoshiiId), method, jsonOrderToPut.ToJsonString());
                }
                else
                {
                    responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order, order.Id), method, jsonOrderToPut.ToJsonString());
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            var dto = new JsonOrder();
            returnOrder = HandleOrderResponse<Models.Order, JsonOrder>(order.Id, responseMessage, out dto);

            return returnOrder;
        }

        /// <summary>
        /// This method is specifically called to confirm an order created on an orderAhead partner.
        /// </summary>
        /// <param name="order">
        /// The order to be confirmed
        /// </param>
        /// <returns>
        /// The order that was returned from the PUT request to Doshii. 
        /// </returns>
        internal virtual Models.Order PutOrderCreatedResult(Models.Order order)
        {
            var returnOrder = new Models.Order();
            DoshiHttpResponseMessage responseMessage;

            try
            {
                var jsonOrderToPut = Mapper.Map<JsonUnlinkedOrderToPut>(order);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders, order.DoshiiId), WebRequestMethods.Http.Put, jsonOrderToPut.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            var dto = new JsonOrder();
            if (order.Status != "rejected")
            {
                returnOrder = HandleOrderResponse<Models.Order, JsonOrder>(order.Id, responseMessage, out dto);
            }
            
            return returnOrder;
        }

        /// <summary>
        /// This function takes the supplied <paramref name="responseMessage"/> received from the RESTful Doshii API and translates it
        /// into some sort of order object. It utilises the mapping between a model object (<typeparamref name="T"/>) and its corresponding 
        /// JSON data transfer object (<typeparamref name="DTO"/>). The data transfer object type should be an extension of the 
        /// <see cref="DoshiiDotNetIntegration.Models.Json.JsonSerializationBase<TSelf>"/> class.
        /// </summary>
        /// <remarks>
        /// The purpose of this function is to provide a consistent manner of parsing the response to the <c>PUT /orders/:pos_id</c> call in the 
        /// API, regardless of the actual model object we are dealing with for the action taken.
        /// </remarks>
        /// <typeparam name="T">The type of model object to be returned by this call. This should be a member of the <c>DoshiiDotNetIntegration.Models</c>
        /// namespace that is mapped to the <typeparamref name="DTO"/> type via the <see cref="DoshiiDotNetIntegration.Helpers.AutoMapperConfigurator"/>
        /// helper class.</typeparam>
        /// <typeparam name="DTO">The corresponding data type object used by the communication with the API for the action.</typeparam>
        /// <param name="orderId">The POS identifier for the order.</param>
        /// <param name="responseMessage">The current response message to be parsed.</param>
        /// <param name="jsonDto">When this function returns, this output parameter will be the data transfer object used in communication with the API.</param>
        /// <returns>The details of the order in the Doshii API.</returns>
        internal T HandleOrderResponse<T, DTO>(string orderId, DoshiHttpResponseMessage responseMessage, out DTO jsonDto)
        {
            jsonDto = default(DTO); // null since its an object
            T returnObj = default(T); // null since its an object

            _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message has been returned to the put order function"));

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response order data was not null"));
                        jsonDto = JsonConvert.DeserializeObject<DTO>(responseMessage.Data);
                        returnObj = Mapper.Map<T>(jsonDto);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                throw new NullResponseDataReturnedException();
            }

            UpdateOrderVersion<T>(returnObj);
            UpdateOrderCheckin<T>(returnObj);


            return returnObj;
        }

        /// <summary>
        /// A call to this function updates the order version in the POS. The generic nature of this function is due to the fact that
        /// we might be dealing with different actual model objects. This function can be used to update the POS version of the order
        /// regardless of the actual type used.
        /// </summary>
        /// <remarks>
        /// NOTE: The SDK implementer must update this call for any new model types that make use of the order version.
        /// </remarks>
        /// <typeparam name="T">The type of model object being updated. In this case, the type should be a derivative of an
        /// <see cref="DoshiiDotNetIntegration.Models.Order"/> or a class that contains a reference to an order.</typeparam>
        /// <param name="orderDetails">The details of the order.</param>
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
        /// This method is used to confirm or reject or update an order when the order has an OrderId
        /// </summary>
        /// <param name="order">
        /// The order to be updated
        /// </param>
        /// <returns>
        /// If the request is not successful a new order will be returned - you can check the order.Id in the returned order to confirm it is a valid response. 
        /// </returns>
        internal virtual Models.Order PutOrder(Models.Order order)
        {
            return PutPostOrder(order, WebRequestMethods.Http.Put);
        }

        
        /// <summary>
        /// Deletes a table allocation from doshii for the provided checkinId. 
        /// </summary>
        /// <returns>
        /// true if successful
        /// false if failed
        /// </returns>
        ///<exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual bool DeleteTableAllocation(string checkinId)
        {
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId), DeleteMethod);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: A 'DELETE' request to {0} was successful. Allocations have been removed", GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId)));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' to URL '{0}'", GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId)));
            }

            return false;
        }
        
        #endregion

        #region Partner App Methods

        internal virtual IEnumerable<Models.App> GetApps()
        {
            var retreivedAppList = new List<Models.App>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.App), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonApp>>(responseMessage.Data);
                        retreivedAppList = Mapper.Map<List<Models.App>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.App)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.App)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.App)));
            }

            return retreivedAppList;
        }
        
        #endregion

        #region transaction methods

        /// <summary>
        /// This method is used to retrieve a list of transaction related to an order with the doshiiOrderId
        /// This method will only retreive transactions for unlinked orders on Doshii - if the order is linked to a pos order there is no method to retreive the transaction in the OrderAhead implementation. 
        /// </summary>
        /// <param name="doshiiOrderId">
        /// the doshiiOrderId for the order. 
        /// </param>
        /// <returns>
        /// A list of transactions associated with the order,
        /// If there are no transaction associated with the order an empty list is returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual IEnumerable<Transaction> GetTransactionsFromDoshiiOrderId(string doshiiOrderId)
        {
            var retreivedTransactionList = new List<Transaction>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTransaction>>(responseMessage.Data);
                        retreivedTransactionList = Mapper.Map<List<Transaction>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId)));
            }

            return retreivedTransactionList;
        }

        /// <summary>
        /// This method is used to retrieve a list of transaction related to an order with the posOrderId
        /// </summary>
        /// <param name="posOrderId">
        /// the posOrderId for the order. 
        /// </param>
        /// <returns>
        /// A list of transactions associated with the order,
        /// If there are no transaction associated with the order an empty list is returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual IEnumerable<Transaction> GetTransactionsFromPosOrderId(string posOrderId)
        {
            var retreivedTransactionList = new List<Transaction>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.TransactionFromPosOrderId, posOrderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTransaction>>(responseMessage.Data);
                        retreivedTransactionList = Mapper.Map<List<Transaction>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.TransactionFromPosOrderId, posOrderId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.TransactionFromPosOrderId, posOrderId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.TransactionFromPosOrderId, posOrderId)));
            }

            return retreivedTransactionList;
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
        internal virtual Transaction GetTransaction(string transactionId)
        {
            var retreivedTransaction = new Transaction();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonTransaction.deseralizeFromJson(responseMessage.Data);
                        retreivedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId)));
            }

            return retreivedTransaction;
        }

        /// <summary>
        /// Gets all the current active transactions in Doshii. 
        /// </summary>
        /// <returns>
        /// an IEnumerable of transactions 
        /// it will be empty if no transactions exist. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual IEnumerable<Transaction> GetTransactions()
        {
            var retreivedTransactionList = new List<Transaction>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Transaction), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTransaction>>(responseMessage.Data);
                        retreivedTransactionList = Mapper.Map<List<Transaction>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction)));
            }

            return retreivedTransactionList;
        }

        /// <summary>
        /// completes the Post request to create a transaction on Doshii. 
        /// </summary>
        /// <param name="transaction">the transaction to be created on Doshii</param>
        /// <returns>
        /// The transaction that was created on Doshii
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Transaction PostTransaction(Transaction transaction)
        {
            DoshiHttpResponseMessage responseMessage;
            Transaction returnedTransaction = null;
            try
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Transaction, transaction.Id), WebRequestMethods.Http.Post, jsonTransaction.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonConvert.DeserializeObject<JsonTransaction>(responseMessage.Data);
                        returnedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                        if (returnedTransaction != null)
                        {
                            _controllersCollection.TransactionController.RecordTransactionVersion(returnedTransaction);
                        }
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                throw new NullResponseDataReturnedException();
            }

            return returnedTransaction;
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
        internal virtual Transaction PutTransaction(Transaction transaction)
        {
            DoshiHttpResponseMessage responseMessage;
            Transaction returnedTransaction = null;
            try
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Transaction, transaction.Id), WebRequestMethods.Http.Put, jsonTransaction.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonConvert.DeserializeObject<JsonTransaction>(responseMessage.Data);
                        returnedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                        if (returnedTransaction != null)
                        {
                            _controllersCollection.TransactionController.RecordTransactionVersion(returnedTransaction);
                        }
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                throw new NullResponseDataReturnedException();
            }

            return returnedTransaction;
        }
#endregion

        #region Member methods

        internal virtual Member GetMember(string memberId)
        {
            var retreivedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Members, memberId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMember = JsonMember.deseralizeFromJson(responseMessage.Data);
                        retreivedMember = Mapper.Map<Member>(jsonMember);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Members, memberId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Members, memberId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Members, memberId)));
            }

            return retreivedMember;
        }

        internal virtual IEnumerable<Member> GetMembers()
        {
            var retreivedMemberList = new List<Member>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonMember>>(responseMessage.Data);
                        retreivedMemberList = Mapper.Map<List<Member>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Members)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Members)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Members)));
            }

            List<Member> fullMemberList = new List<Member>();
            foreach (var partmember in retreivedMemberList)
            {
                Member newMember = GetMember(partmember.Id);
                fullMemberList.Add(newMember);
            }
            return (IEnumerable<Member>)fullMemberList;
        }


        internal virtual Member PutMember(Member member)
        {
            var returnedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonMember = Mapper.Map<JsonMemberToUpdate>(member);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members, member.Id), WebRequestMethods.Http.Put, jsonMember.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMember = JsonConvert.DeserializeObject<JsonMember>(responseMessage.Data);
                        returnedMember = Mapper.Map<Member>(jsonMember);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Members, member.Id)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.Members, member.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.Members, member.Id)));
                throw new NullResponseDataReturnedException();
            }
            return returnedMember;
        }

        internal virtual Member PostMember(Member member)
        {
            var returnedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonMember = Mapper.Map<JsonMemberToUpdate>(member);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members), WebRequestMethods.Http.Post, jsonMember.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMember = JsonConvert.DeserializeObject<JsonMember>(responseMessage.Data);
                        returnedMember = Mapper.Map<Member>(jsonMember);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Members)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.Members)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.Members)));
                throw new NullResponseDataReturnedException();
            }
            return returnedMember;
        }

        internal virtual bool DeleteMember(Member member)
        {
            var returnedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members, member.Id), HttpController.DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                    

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(EndPointPurposes.Members, member.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' and URL '{0}'", GenerateUrl(EndPointPurposes.Members, member.Id)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }

        internal virtual IEnumerable<Reward> GetRewardsForMember(string memberId, string orderId, decimal orderTotal)
        {
            var retreivedRewardList = new List<Reward>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(string.Format("{0}?orderId={1}&orderTotal={2}", GenerateUrl(EndPointPurposes.MemberRewards, memberId),orderId,orderTotal), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonReward>>(responseMessage.Data);
                        retreivedRewardList = Mapper.Map<List<Reward>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.MemberRewards, memberId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.MemberRewards, memberId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.MemberRewards, memberId)));
            }

            return retreivedRewardList;
        }

        internal virtual bool RedeemRewardForMember(string memberId, string rewardId, Models.Order order)
        {
            
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonOrderIdSimple = Mapper.Map<JsonOrderIdSimple>(order);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberRewardsRedeem, memberId, rewardId), WebRequestMethods.Http.Post, jsonOrderIdSimple.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberRewardsRedeem, memberId, rewardId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberRewardsRedeem, memberId, rewardId)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }

        internal virtual bool RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {

            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberRewardsRedeemCancel, memberId, rewardId), WebRequestMethods.Http.Put, "{ \"reason\": \""+cancelReason+"\"}");
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberRewardsRedeemCancel, memberId, rewardId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberRewardsRedeemCancel, memberId, rewardId)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }

        internal virtual bool RedeemRewardForMemberConfirm(string memberId, string rewardId)
        {

            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberRewardsRedeemConfirm, memberId, rewardId), WebRequestMethods.Http.Put);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberRewardsRedeemConfirm, memberId, rewardId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberRewardsRedeemConfirm, memberId, rewardId)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }

        internal virtual bool RedeemPointsForMember(PointsRedeem pr, Member member)
        {
            DoshiHttpResponseMessage responseMessage;
            //create redeem points object
            try
            {
                var jsonPointsRedeem = Mapper.Map<JsonPointsRedeem>(pr);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberPointsRedeem, member.Id), WebRequestMethods.Http.Post, jsonPointsRedeem.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberPointsRedeem, member.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberPointsRedeem, member.Id)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }

        internal virtual bool RedeemPointsForMemberConfirm(string memberId)
        {
            DoshiHttpResponseMessage responseMessage;
            //create redeem points object
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberPointsRedeemConfirm, memberId), WebRequestMethods.Http.Put);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberPointsRedeemConfirm, memberId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberPointsRedeemConfirm, memberId)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }

        internal virtual bool RedeemPointsForMemberCancel(string memberId, string cancelReason)
        {
            DoshiHttpResponseMessage responseMessage;
            //create redeem points object
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberPointsRedeemCancel, memberId), WebRequestMethods.Http.Put, "{ \"reason\": \"" + cancelReason + "\"}");
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberPointsRedeemCancel, memberId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberPointsRedeemCancel, memberId)));
                throw new NullResponseDataReturnedException();
            }
            return false;
        }
        #endregion

        #region checkins / consumers
        /// <summary>
        /// This method is use to get a consumer from Doshii that corresponds with the checkinId 
        /// </summary>
        /// <param name="checkinId">
        /// The checkinId identifying the consumer. 
        /// </param>
        /// <returns>
        /// The consumer returned by doshii
        /// or a blank consumer if no consumer was returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Consumer GetConsumerFromCheckinId(string checkinId)
        {
            var retreivedConsumer = new Consumer();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonConsumer = JsonConsumer.deseralizeFromJson(responseMessage.Data);
                        retreivedConsumer = Mapper.Map<Consumer>(jsonConsumer);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId)));
            }

            return retreivedConsumer;
        }

        internal virtual Checkin PostCheckin(Checkin checkin)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins), WebRequestMethods.Http.Post, jsonCheckin.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonCheckin.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins)));
            }

            return retreivedCheckin;
        }

        internal virtual Checkin PutCheckin(Checkin checkin)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id), WebRequestMethods.Http.Put, jsonCheckin.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw new CheckinUpdateException("Exception updating checkin", rex);
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonCheckin.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id)));
            }

            return retreivedCheckin;
        }

        internal virtual Checkin DeleteCheckin(string checkinId)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId), DeleteMethod);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonCheckin.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
            }

            return retreivedCheckin;
        }

        internal virtual Checkin GetCheckin(string checkinId)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonCheckin.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
            }

            return retreivedCheckin;
        }

        internal virtual IEnumerable<Checkin> GetCheckins()
        {
            var retreivedCheckinList = new List<Checkin>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<Checkin>>(responseMessage.Data);
                        retreivedCheckinList = Mapper.Map<List<Checkin>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins)));
            }

            return retreivedCheckinList;
        }

#endregion

        #region Menu
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
        internal virtual Menu PostMenu(Menu menu)
        {
            var returedMenu = new Menu();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonMenu = Mapper.Map<JsonMenu>(menu);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Menu), WebRequestMethods.Http.Post, jsonMenu.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMenu = JsonConvert.DeserializeObject<JsonMenu>(responseMessage.Data);
                        returedMenu = Mapper.Map<Menu>(jsonMenu);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Menu)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Menu)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Menu)));
                throw new NullResponseDataReturnedException();
            }
            return returedMenu;
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
        internal virtual Surcount PutSurcount(Surcount surcount)
        {
            var returedSurcount = new Surcount();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonSurcount = Mapper.Map<JsonMenuSurcount>(surcount);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Surcounts, surcount.Id), WebRequestMethods.Http.Put, jsonSurcount.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonSurcount = JsonConvert.DeserializeObject<JsonMenuSurcount>(responseMessage.Data);
                        returedSurcount = Mapper.Map<Surcount>(jsonSurcount);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Surcounts, surcount.Id)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Surcounts, surcount.Id)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Surcounts, surcount.Id)));
                throw new NullResponseDataReturnedException();
            }
            return returedSurcount;
        }

        /// <summary>
        /// Deletes a surcount from the pos menu on doshii
        /// </summary>
        /// <param name="posId">
        /// The pos Id related to the surcount on Doshii
        /// </param>
        /// <returns></returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual bool DeleteSurcount(string posId)
        {
            var returedMenu = new Menu();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Surcounts, posId), DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage.Status == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
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
        internal virtual Product PutProduct(Product product)
        {
            var returnedProduct = new Product();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonProduct = Mapper.Map<JsonMenuProduct>(product);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Products, product.PosId), WebRequestMethods.Http.Put, jsonProduct.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonProduct = JsonConvert.DeserializeObject<JsonMenuProduct>(responseMessage.Data);
                        returnedProduct = Mapper.Map<Product>(jsonProduct);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Products, product.PosId)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.Products, product.PosId)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.Products, product.PosId)));
                throw new NullResponseDataReturnedException();
            }
            return returnedProduct;
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
        internal virtual bool DeleteProduct(string posId)
        {
            var returedMenu = new Menu();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Products, posId), DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage.Status == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
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
        internal virtual Location GetLocation(string hashedLocationId = "")
        {
            var retreivedLocation = new Location();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                if (string.IsNullOrEmpty(hashedLocationId))
                {
                    responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Location), WebRequestMethods.Http.Get);
                }
                else
                {
                    responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Location), WebRequestMethods.Http.Get, hashedLocationId);
                }
                
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonLocation = JsonLocation.deseralizeFromJson(responseMessage.Data);
                        retreivedLocation = Mapper.Map<Location>(jsonLocation);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Location)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Location)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Location)));
            }

            return retreivedLocation;
        }

        internal virtual Location PostLocation(Location location)
        {
            var retreivedLocation = new Location();
            var locationToPost = Mapper.Map<JsonEmployee>(location);
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Location), WebRequestMethods.Http.Get, locationToPost.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonLocation = JsonLocation.deseralizeFromJson(responseMessage.Data);
                        retreivedLocation = Mapper.Map<Location>(jsonLocation);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Location)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Location)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Location)));
            }

            return retreivedLocation;
        }

        internal virtual IEnumerable<Models.Location> GetLocations()
        {
            var retreivedLocationList = new List<Models.Location>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Location), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<Location>>(responseMessage.Data);
                        retreivedLocationList = Mapper.Map<List<Models.Location>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Location)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Location)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Location)));
            }

            return retreivedLocationList;
        }

        internal virtual Orginisation PostOrginisation(Orginisation orginisation)
        {
            var retreivedOrginisation = new Orginisation();
            var orginisationToPost = Mapper.Map<JsonOrginisation>(orginisation);
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Orginisation), WebRequestMethods.Http.Get, orginisationToPost.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonLocation = JsonOrginisation.deseralizeFromJson(responseMessage.Data);
                        retreivedOrginisation = Mapper.Map<Orginisation>(jsonLocation);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Orginisation)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Orginisation)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Orginisation)));
            }

            return retreivedOrginisation;
        }

#endregion

        #region Tables

        internal virtual Table PostTable(Table table)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                var jsonTable = Mapper.Map<JsonTable>(table);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables), WebRequestMethods.Http.Post, jsonTable.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Tables)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Tables)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Tables)));
                throw new NullResponseDataReturnedException();
            }

            return returnedTable;
        }

        internal virtual Table PutTable(Table table, string oldTableName)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                var jsonTable = Mapper.Map<JsonTable>(table);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables, oldTableName), WebRequestMethods.Http.Put, jsonTable.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Tables, table.Name)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Tables, table.Name)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Tables, table.Name)));
                throw new NullResponseDataReturnedException();
            }

            return returnedTable;
        }

        internal virtual List<Table> PutTables(List<Table> tables)
        {
            DoshiHttpResponseMessage responseMessage;
            List<Table> retreivedtableList = null;
            try
            {
                List<JsonTable> jsonTableList = new List<JsonTable>();
                foreach (var t in tables)
                {
                    jsonTableList.Add(Mapper.Map<JsonTable>(t));
                }
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables), WebRequestMethods.Http.Put, JsonConvert.SerializeObject(jsonTableList));
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        if (responseMessage.Data != "[]")
                        {
                            var jsonList = JsonConvert.DeserializeObject<List<JsonTable>>(responseMessage.Data);
                            retreivedtableList = Mapper.Map<List<Table>>(jsonList);
                        }

                        
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Tables)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Tables)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Tables)));
                throw new NullResponseDataReturnedException();
            }

            return retreivedtableList;
        }

        internal virtual Table DeleteTable(string tableName)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables, tableName), DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Tables, tableName)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(EndPointPurposes.Tables, tableName)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' and URL '{0}'", GenerateUrl(EndPointPurposes.Tables, tableName)));
                throw new NullResponseDataReturnedException();
            }

            return returnedTable;
        }

        internal virtual Table GetTable(string tableName)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables, tableName), WebRequestMethods.Http.Get);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Tables, tableName)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(EndPointPurposes.Tables, tableName)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(EndPointPurposes.Tables, tableName)));
                throw new NullResponseDataReturnedException();
            }

            return returnedTable;
        }

        internal virtual IEnumerable<Table> GetTables()
        {
            var retreivedtableList = new List<Table>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTable>>(responseMessage.Data);
                        retreivedtableList = Mapper.Map<List<Table>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Tables)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(EndPointPurposes.Tables)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(EndPointPurposes.Tables)));
            }

            return retreivedtableList;
        }

        #endregion

        #region Bookings

        internal Checkin SeatBooking(string bookingId, Checkin checkin)
        {
            DoshiHttpResponseMessage responseMessage;
            var retreivedCheckin = new Checkin();

            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.BookingsCheckin, bookingId), WebRequestMethods.Http.Post, jsonCheckin.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonCheckin.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.BookingsCheckin)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.BookingsCheckin)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.BookingsCheckin)));
            }

            return retreivedCheckin;
        }

        internal virtual Booking GetBooking(String bookingId)
        {
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Booking, bookingId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonBooking = JsonConvert.DeserializeObject<JsonBooking>(responseMessage.Data);
                        return Mapper.Map<Booking>(jsonBooking);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Booking)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(EndPointPurposes.Booking)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(EndPointPurposes.Booking)));
            }
            return null;
        }

        internal virtual IEnumerable<Booking> GetBookings(DateTime from, DateTime to)
        {
            List<Booking> retrievedBookings = new List<Booking>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Bookings, from.ToEpochSeconds().ToString(), to.ToEpochSeconds().ToString()), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonBooking>>(responseMessage.Data);
                        retrievedBookings = Mapper.Map<List<Booking>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Bookings)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(EndPointPurposes.Bookings)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(EndPointPurposes.Bookings)));
            }
            return retrievedBookings;
        }
        #endregion

        #region RejectionCodes

        internal virtual IEnumerable<Models.RejectionCode> GetRejectionCodes()
        {
            var retreivedAppList = new List<Models.RejectionCode>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.RejectionCodes), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonRejectionCode>>(responseMessage.Data);
                        retreivedAppList = Mapper.Map<List<Models.RejectionCode>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.RejectionCodes)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.RejectionCodes)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.RejectionCodes)));
            }

            return retreivedAppList;
        }

        internal virtual Models.RejectionCode GetRejectionCode(string code)
        {
            var retreivedRejectionCode = new Models.RejectionCode();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.RejectionCodes), WebRequestMethods.Http.Get, code);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<JsonRejectionCode>(responseMessage.Data);
                        retreivedRejectionCode = Mapper.Map<Models.RejectionCode>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.RejectionCodes)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.RejectionCodes)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.RejectionCodes)));
            }

            return retreivedRejectionCode;
        }
        
        #endregion

        #region Employee

        internal virtual IEnumerable<Models.Employee> GetEmployees()
        {
            var retreivedEmployeeList = new List<Models.Employee>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Employee), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonEmployee>>(responseMessage.Data);
                        retreivedEmployeeList = Mapper.Map<List<Models.Employee>>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Employee)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Employee)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Employee)));
            }

            return retreivedEmployeeList;
        }

        internal virtual Models.Employee GetEmployee(string doshiiId)
        {
            var retreivedEmployee = new Models.Employee();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Employee), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<JsonEmployee>(responseMessage.Data);
                        retreivedEmployee = Mapper.Map<Models.Employee>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Employee)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Employee)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Employee)));
            }

            return retreivedEmployee;
        }

        internal virtual Models.Employee PostEmployee(Employee employee)
        {
            var retreivedEmployee = new Models.Employee();
            var employeeToPost = Mapper.Map<JsonEmployee>(employee);
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Employee), WebRequestMethods.Http.Post, employeeToPost.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<JsonEmployee>(responseMessage.Data);
                        retreivedEmployee = Mapper.Map<Models.Employee>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Employee)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Employee)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Employee)));
            }

            return retreivedEmployee;
        }

        internal virtual Models.Employee PutEmployee(Employee employee)
        {
            var retreivedEmployee = new Models.Employee();
            var employeeToPost = Mapper.Map<JsonEmployee>(employee);
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Employee), WebRequestMethods.Http.Put, employeeToPost.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<JsonEmployee>(responseMessage.Data);
                        retreivedEmployee = Mapper.Map<Models.Employee>(jsonList);
                    }
                    else
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Employee)));
                    }

                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Employee)));
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Employee)));
            }

            return retreivedEmployee;
        }

        internal virtual bool DeleteEmployee(Employee employee)
        {
            var retreivedEmployee = new Models.Employee();
            var employeeToPost = Mapper.Map<JsonEmployee>(employee);
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Employee), HttpController.DeleteMethod, employee.Id);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Employee)));
                    return false;
                }
            }
            else
            {
                _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Employee)));
                return false;
            }

            return false;
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
        /// eg, the orderId for a get order request
        /// </param>
        /// <returns>
        /// The Url required to make the desiered request. 
        /// </returns>
        internal virtual string GenerateUrl(EndPointPurposes purpose, string identification = "", string secondIdentification = "")
        {
            StringBuilder newUrlbuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_doshiiUrlBase))
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error, "Doshii: The HttpController class was not initialized correctly, the base URl is null or white space");
                return newUrlbuilder.ToString();
            }
            newUrlbuilder.AppendFormat("{0}", _doshiiUrlBase);

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
                    newUrlbuilder.AppendFormat("/bookings/{0}", identification);
                    break;
                case EndPointPurposes.Bookings:
                    newUrlbuilder.Append("/bookings");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("?from={0}&to={1}", identification, secondIdentification);
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
                default:
                    throw new NotSupportedException(purpose.ToString());
            }

            return newUrlbuilder.ToString();
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
        /// eg, a JSON representation of the order that should be send to Doshii with a PUT order request. 
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
        internal virtual DoshiHttpResponseMessage MakeRequest(string url, string method, string data = "")
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
            request.Headers.Add("authorization", AuthHelper.CreateToken(_controllersCollection.ConfigurationManager.GetLocationTokenFromPos(), _controllersCollection.ConfigurationManager.GetSecretKeyFromPos()));
            request.Headers.Add("vendor", _controllersCollection.ConfigurationManager.GetVendorFromPos());
            request.ContentType = "application/json";


            if (method.Equals(WebRequestMethods.Http.Get) || 
				method.Equals(WebRequestMethods.Http.Put) || 
				method.Equals(WebRequestMethods.Http.Post) || 
				method.Equals(HttpController.DeleteMethod))
            {
                request.Method = method;
            }
            else
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error, string.Format("MakeRequest was called with a non supported HTTP request method type - '{0}", method));
                throw new NotSupportedException("Invalid HTTP request Method Type");
            }
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
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: generating {0} request to endpoint {1}, with data {2}", method, url, data));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                responceMessage.Status = response.StatusCode;
                responceMessage.StatusDescription = responceMessage.StatusDescription;

                StreamReader sr = new StreamReader(response.GetResponseStream());
                responceMessage.Data = sr.ReadToEnd();

                sr.Close();
                response.Close();

                if (responceMessage.Status == HttpStatusCode.OK || responceMessage.Status == HttpStatusCode.Created)
                {
					_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Info, string.Format("Doshii: Successful response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                else if (responceMessage.Status == HttpStatusCode.BadRequest || 
                    responceMessage.Status == HttpStatusCode.Unauthorized || 
                    responceMessage.Status == HttpStatusCode.Forbidden || 
                    responceMessage.Status == HttpStatusCode.InternalServerError || 
                    responceMessage.Status == HttpStatusCode.NotFound || 
                    responceMessage.Status == HttpStatusCode.Conflict ||
                    responceMessage.Status == (HttpStatusCode)456) //Upstream rejected
                {
					_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                    throw new Exceptions.RestfulApiErrorResponseException(responceMessage.Status, responceMessage.Message);
                }
                else
                {
					_controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
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
                        if (httpResponse.StatusCode == HttpStatusCode.BadRequest ||
                            httpResponse.StatusCode == HttpStatusCode.Unauthorized ||
                            httpResponse.StatusCode == HttpStatusCode.Forbidden ||
                            httpResponse.StatusCode == HttpStatusCode.InternalServerError ||
                            httpResponse.StatusCode == HttpStatusCode.NotFound ||
                            httpResponse.StatusCode == HttpStatusCode.Conflict)
                        {
                            _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error,
                                string.Format(
                                    "Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}",
                                    method, url, data, errorResponce, wex));
                            throw new Exceptions.RestfulApiErrorResponseException(httpResponse.StatusCode, errorResponce, wex);
                        }
                        else
                        {
                            _controllersCollection.LoggingController.LogMessage(typeof(HttpController), DoshiiLogLevels.Error,
                                string.Format(
                                    "Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}",
                                    method, url, data, errorResponce, wex));
                        }
                    }
                }
                else
                {
                    throw new Exceptions.RestfulApiErrorResponseException("There was no response in the web exception while making a request.");
                }
                
            }
            catch (Exception ex)
            {
				_controllersCollection.LoggingController.LogMessage(typeof(HttpController), Enums.DoshiiLogLevels.Error, string.Format("Doshii: As exception was thrown while attempting a {0} request to endpoint {1}, with data {2} and status {3} : {4}", method, url, data, responceMessage.Status.ToString(), ex));
				throw new Exceptions.RestfulApiErrorResponseException(responceMessage.Status, ex);
            }

            return responceMessage;
        }

        

#endregion

    }
}
