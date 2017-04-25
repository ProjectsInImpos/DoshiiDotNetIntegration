﻿using System;
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
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// This class is used internally by the SDK to manage the SDK to manage the business logic handling transactions.
    /// </summary>
    internal class TransactionController
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
        /// <param name="transactionManager"></param>
        /// <param name="httpComs"></param>
        /// <param name="controllerCollection"></param>
        internal TransactionController(Models.ControllersCollection controllerCollection, HttpController httpComs)
        {
            if (controllerCollection == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllersCollection = controllerCollection;
            if (controllerCollection.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            _controllersCollection.LoggingController = controllerCollection.LoggingController;
            if (_controllersCollection.TransactionManager == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - transactionManager cannot be null");
                throw new NullReferenceException("transactionManager cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
        }

        /// <summary>
        /// calls the appropriate callback method on <see cref="ITransactionManager"/> to record the order version.
        /// </summary>
        /// <param name="transaction">
        /// the transaction to be recorded
        /// </param>
        internal virtual void RecordTransactionVersion(Transaction transaction)
        {
            try
            {
                _controllersCollection.TransactionManager.RecordTransactionVersion(transaction.Id, transaction.Version);
            }
            catch (TransactionDoesNotExistOnPosException nex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, string.Format("Doshii: Attempted to update a transaction version for a transaction that does not exist on the Pos, TransactionId - {0}, version - {1}", transaction.Id, transaction.Version));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update a transaction version on the pos, TransactionId - {0}, version - {1}, {2}", transaction.Id, transaction.Version, ex.ToString()));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
            }
        }

        /// <summary>
        /// Attempts to add a pos transaction to doshii
        /// </summary>
        /// <param name="transaction">
        /// The transaction to add to Doshii
        /// </param>
        /// <returns>
        /// The transaction that was recorded on doshii if the request was successful
        /// <para/>Returns null if the request failed. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        internal virtual Transaction RecordPosTransactionOnDoshii(Transaction transaction)
        {
            Transaction returnedTransaction;
            try
            {
                returnedTransaction = _httpComs.PostTransaction(transaction);
            }
            catch (Exception ex)
            {
                return null;
            }
            return returnedTransaction;
        }

        /// <summary>
        /// gets a transaction from Doshii from the provided transactionId
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        internal virtual Transaction GetTransaction(string transactionId)
        {
            try
            {
                return _httpComs.GetTransaction(transactionId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// get a list of transactions from Doshii related to the DoshiiOrderId provided, 
        /// This method is primarily used to get the transactions for unlinked orders before the pos is alerted to the creation of the order. 
        /// </summary>
        /// <param name="doshiiOrderId"></param>
        /// <returns></returns>
        internal virtual IEnumerable<Transaction> GetTransactionFromDoshiiOrderId(string doshiiOrderId)
        {
            try
            {
                return _httpComs.GetTransactionsFromDoshiiOrderId(doshiiOrderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                //this means there were no transactions for the unlinked order. 
                if (rex.StatusCode == HttpStatusCode.NotFound)
                {
                    List<Transaction> emplyTransactionList = new List<Transaction>();
                    return emplyTransactionList;
                }
                else
                {
                    throw rex;
                }
            }
        }

        /// <summary>
        /// get a list of transactions from Doshii related to the posOrderId provided, 
        /// </summary>
        /// <param name="doshiiOrderId"></param>
        /// <returns></returns>
        internal virtual IEnumerable<Transaction> GetTransactionFromPosOrderId(string posOrderId)
        {
            try
            {
                return _httpComs.GetTransactionsFromPosOrderId(posOrderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                //this means there were no transactions for the unlinked order. 
                if (rex.StatusCode == HttpStatusCode.NotFound)
                {
                    List<Transaction> emplyTransactionList = new List<Transaction>();
                    return emplyTransactionList;
                }
                else
                {
                    throw rex;
                }
            }
        }

        /// <summary>
        /// Gets all the open transactions from Doshii. 
        /// </summary>
        /// <returns></returns>
        internal virtual IEnumerable<Transaction> GetTransactions()
        {
            try
            {
                return _httpComs.GetTransactions();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method requests a payment from Doshii
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="transaction">
        /// The transaction that should be paid
        /// </param>
        /// <returns>
        /// True on successful payment; false otherwise.
        /// </returns>
        internal virtual bool RequestPaymentForOrderExistingTransaction(Transaction transaction)
        {
            var returnedTransaction = new Transaction();
            transaction.Status = "waiting";

            try
            {
                //as the transaction cannot currently be changed on doshii and transacitons are only created when a payment is made with an order the line below is not necessary unitll
                //doshii is enhanced to allow modifying of transactions. 
                //transaction.Version = TransactionManager.RetrieveTransactionVersion(transaction.Id);
                returnedTransaction = _httpComs.PutTransaction(transaction);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.NotFound)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: The partner could not locate the transaction for order.Id{0}", transaction.OrderId), rex);
                }
                else if (rex.StatusCode == HttpStatusCode.PaymentRequired)
                {
                    // this just means that the partner failed to claim payment when requested
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error,
                        string.Format("Doshii: The partner could not claim the payment for for order.Id{0}", transaction.OrderId), rex);
                }
                else
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error,
                        string.Format("Doshii: There was an unknown exception while attempting to get a payment from doshii"), rex);
                }
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return false;
            }
            catch (NullResponseDataReturnedException)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a postTransaction for order.Id{0}", transaction.OrderId));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return false;
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a postTransaction for order.Id {0} : {1}", transaction.OrderId, ex));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return false;
            }

            if (returnedTransaction != null && returnedTransaction.Id == transaction.Id)
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: transaction post for payment - '{0}'", jsonTransaction.ToJsonString()));
                //returnedTransaction.OrderId = transaction.OrderId;
                _controllersCollection.TransactionManager.RecordSuccessfulPayment(returnedTransaction);
                _controllersCollection.TransactionManager.RecordTransactionVersion(returnedTransaction.Id, returnedTransaction.Version);
                return true;
            }
            else
            {
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return false;
            }
        }

        /// <summary>
        /// This method requests a payment from Doshii
        /// calls <see cref="m_DoshiiInterface.CheckOutConsumerWithCheckInId"/> when order update was reject by doshii for a reason that means it should not be retired. 
        /// calls <see cref="m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordPartialCheckPayment(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordFullCheckPayment(ref order)"/>
        /// to record the payment in the pos. 
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="transaction">
        /// The order that should be paid
        /// </param>
        /// <returns>
        /// True on successful payment; false otherwise.
        /// </returns>
        internal virtual bool RejectPaymentForOrder(Transaction transaction)
        {
            var returnedTransaction = new Transaction();
            transaction.Status = "rejected";

            try
            {
                returnedTransaction = _httpComs.PutTransaction(transaction);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: The partner could not locate the transaction for transaction.Id{0}", transaction.OrderId));
                return false;

            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putTransaction for transaction.Id {0} : {1}", transaction.OrderId, ex));
                return false;
            }

            if (returnedTransaction != null && returnedTransaction.Id == transaction.Id && returnedTransaction.Status == "complete")
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: transaction put for payment - '{0}'", jsonTransaction.ToJsonString()));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handels the Pending transaction received event by calling the appropriate callback methods on the <see cref="ITransactionManager"/> Interface. 
        /// </summary>
        /// <param name="receivedTransaction">
        /// The pending transaction that needs to be processed. 
        /// </param>
        internal virtual void HandelPendingTransactionReceived(Transaction receivedTransaction)
        {
            Transaction transactionFromPos = null;
            try
            {
                transactionFromPos = _controllersCollection.TransactionManager.ReadyToPay(receivedTransaction);
            }
            catch (OrderDoesNotExistOnPosException)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: A transaction was initiated on the Doshii API for an order that does not exist on the system, orderid {0}", receivedTransaction.OrderId));
                receivedTransaction.Status = "rejected";
                RejectPaymentForOrder(receivedTransaction);
                return;
            }

            if (transactionFromPos != null)
            {
                _controllersCollection.TransactionManager.RecordTransactionVersion(receivedTransaction.Id, receivedTransaction.Version);
                RequestPaymentForOrderExistingTransaction(transactionFromPos);
            }
            else
            {
                receivedTransaction.Status = "rejected";
                RejectPaymentForOrder(receivedTransaction);
            }
        }
    }
}
