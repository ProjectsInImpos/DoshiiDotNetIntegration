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
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;
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
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - transactionManager cannot be null");
                throw new NullReferenceException("transactionManager cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
        }

        /// <summary>
        /// calls the appropriate callback method on <see cref="ITransactionManager"/> to record the Order version.
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
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, string.Format(" Attempted to update a transaction version for a transaction that does not exist on the Pos, TransactionId - {0}, version - {1}", transaction.Id, transaction.Version));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" Exception while attempting to update a transaction version on the pos, TransactionId - {0}, version - {1}, {2}", transaction.Id, transaction.Version, ex.ToString()));
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
        internal virtual ObjectActionResult<Transaction> RecordPosTransactionOnDoshii(Transaction transaction)
        {
            try
            {
                return _httpComs.PostTransaction(transaction);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// gets a transaction from Doshii from the provided transactionId
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<Transaction> GetTransaction(string transactionId)
        {
            try
            {
                return _httpComs.GetTransaction(transactionId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// get a list of transactions from Doshii related to the DoshiiOrderId provided, 
        /// This method is primarily used to get the transactions for unlinked orders before the pos is alerted to the creation of the Order. 
        /// </summary>
        /// <param name="doshiiOrderId"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<List<Transaction>> GetTransactionFromDoshiiOrderId(string doshiiOrderId)
        {
            try
            {
                return _httpComs.GetTransactionsFromDoshiiOrderId(doshiiOrderId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// get a list of transactions from Doshii related to the posOrderId provided, 
        /// </summary>
        /// <param name="doshiiOrderId"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<List<Transaction>> GetTransactionFromPosOrderId(string posOrderId)
        {
            try
            {
                return _httpComs.GetTransactionsFromPosOrderId(posOrderId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// Gets all the open transactions from Doshii. 
        /// </summary>
        /// <returns></returns>
        internal virtual ObjectActionResult<List<Transaction>> GetTransactions()
        {
            try
            {
                return _httpComs.GetTransactions();
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method requests a payment from Doshii
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an Order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="transaction">
        /// The transaction that should be paid
        /// </param>
        /// <returns>
        /// True on successful payment; false otherwise.
        /// </returns>
        internal virtual ActionResultBasic RequestPaymentForOrderExistingTransaction(Transaction transaction)
        {
            transaction.Status = "waiting";
            var result = RequestTransactionProcessing(transaction);

            if (result.ReturnObject != null && result.ReturnObject.Id == transaction.Id)
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: transaction post for payment - '{0}'", jsonTransaction.ToJsonString()));
                //returnedTransaction.OrderId = transaction.OrderId;
                _controllersCollection.TransactionManager.RecordSuccessfulPayment(result.ReturnObject);
                _controllersCollection.TransactionManager.RecordTransactionVersion(result.ReturnObject.Id, result.ReturnObject.Version);
                return new ActionResultBasic()
                {
                    Success = true
                };
            }
            else
            {
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = string.Format("The transaciton was not returned successfully from Doshii.")
                };
            }
        }

        internal virtual ActionResultBasic RequestPaymentForRefund(Transaction transaction)
        {
            transaction.Status = "waiting";
            var result = RequestTransactionProcessing(transaction);

            if (result.ReturnObject != null && result.ReturnObject.Id == transaction.Id)
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: transaction post for refund - '{0}'", jsonTransaction.ToJsonString()));
                //returnedTransaction.OrderId = transaction.OrderId;
                _controllersCollection.TransactionManager.RecordSuccessfulRefund(result.ReturnObject);
                _controllersCollection.TransactionManager.RecordTransactionVersion(result.ReturnObject.Id, result.ReturnObject.Version);
                return new ActionResultBasic()
                {
                    Success = true
                };
            }
            else
            {
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = string.Format("The transaciton was not returned successfully from Doshii.")
                };
            }
        }

        internal virtual ObjectActionResult<Transaction> RequestTransactionProcessing(Transaction transaction)
        {
            transaction.Status = "waiting";
            try
            {
                return _httpComs.PutTransaction(transaction);
            }
            catch (NullResponseDataReturnedException ndr)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a Null response was returned during a postTransaction for Order.Id{0}", transaction.OrderId));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return new ObjectActionResult<Transaction>()
                {
                    Success = false,
                    FailReason = string.Format(DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("put transaction"))
                };
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown during a postTransaction for Order.Id {0} : {1}", transaction.OrderId, ex));
                _controllersCollection.TransactionManager.CancelPayment(transaction);
                return new ObjectActionResult<Transaction>()
                {
                    Success = false,
                    FailReason = string.Format(DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("put transaction"))
                };
            }
        }

        /// <summary>
        /// This method requests a payment from Doshii
        /// calls <see cref="m_DoshiiInterface.CheckOutConsumerWithCheckInId"/> when Order update was reject by doshii for a reason that means it should not be retired. 
        /// calls <see cref="m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordPartialCheckPayment(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordFullCheckPayment(ref order)"/>
        /// to record the payment in the pos. 
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an Order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="transaction">
        /// The Order that should be paid
        /// </param>
        /// <returns>
        /// True on successful payment; false otherwise.
        /// </returns>
        internal virtual ActionResultBasic RejectPaymentForOrder(Transaction transaction)
        {
            transaction.Status = "rejected";

            try
            {
                return _httpComs.PutTransaction(transaction);
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown during a putTransaction for transaction.Id {0} : {1}", transaction.OrderId, ex));
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("put transaction")
                };
            }
        }

        internal virtual void HandelPendingRefundTransactionReceived(Transaction receivedTransaction)
        {
            Transaction transactionFromPos = null;
            try
            {
                transactionFromPos = _controllersCollection.TransactionManager.ReadyToRefund(receivedTransaction);
            }
            catch (OrderDoesNotExistOnPosException)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" A transaction was initiated on the Doshii API for an Order that does not exist on the system, orderid {0}", receivedTransaction.OrderId));
                receivedTransaction.Status = "rejected";
                RejectPaymentForOrder(receivedTransaction);
                return;
            }

            if (transactionFromPos != null)
            {
                _controllersCollection.TransactionManager.RecordTransactionVersion(receivedTransaction.Id, receivedTransaction.Version);
                RequestPaymentForRefund(transactionFromPos);
            }
            else
            {
                receivedTransaction.Status = "rejected";
                RejectPaymentForOrder(receivedTransaction);
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
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" A transaction was initiated on the Doshii API for an Order that does not exist on the system, orderid {0}", receivedTransaction.OrderId));
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

        internal virtual ActionResultBasic RequestRefundForOrder(Order order, int amountToRefundCents, List<string> transacitonIdsToRefund)
        {
            ObjectActionResult<Transaction> returnedTransaction = null;
            Transaction refundTransaction = new Transaction()
            {
                AcceptLess = false,
                CreatedAt = null,
                Id = string.Empty,
                OrderId = order.Id,
                Partner = string.Empty,
                PartnerInitiated = false,
                Invoice = string.Empty,
                PaymentAmount = amountToRefundCents,
                Reference = string.Empty,
                LinkedTrxIds = transacitonIdsToRefund
            };
            try
            {
                return RequestPaymentForRefund(refundTransaction); 
            }
            catch (NullResponseDataReturnedException)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a Null response was returned during a postTransaction for Order.Id{0}", refundTransaction.OrderId));
                _controllersCollection.TransactionManager.CancelPayment(refundTransaction);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("post transaction")
                };
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown during a postTransaction for Order.Id {0} : {1}", refundTransaction.OrderId, ex));
                _controllersCollection.TransactionManager.CancelPayment(refundTransaction);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("post transaction")
                };
            }
        }

        
    }
}
