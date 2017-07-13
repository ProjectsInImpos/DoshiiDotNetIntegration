using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using pos.Models;

namespace pos.DoshiiImplementation
{
    public class TransactionManager : ITransactionManager
    {
        public Transaction ReadyToPay(Transaction transaction)
        {
            if (LiveData.PosSettings.ConfirmAllTransactions)
            {
                transaction.Status = "waiting";
            }
            else
            {
                transaction.Status = "rejected";
            }
            return transaction;
        }

        public Transaction ReadyToRefund(Transaction transaction)
        {
            if (LiveData.PosSettings.ConfirmAllRefunds)
            {
                transaction.Status = "waiting";
            }
            else
            {
                transaction.Status = "rejected";
            }
            return transaction;
        }

        public void CancelPayment(Transaction transaction)
        {
            LiveData.TransactionList.RemoveAll(t => t.Id == transaction.Id);
        }

        public void RecordSuccessfulPayment(Transaction transaction)
        {
            LiveData.TransactionList.Add(transaction);
        }

        public void RecordTransactionVersion(string transactionId, string version)
        {
            if (LiveData.TransactionInfoList.Any(t => t.TransactionId == transactionId))
            {
                foreach (var tra in LiveData.TransactionInfoList.Where(t => t.TransactionId == transactionId))
                {
                    tra.Version = version;
                }
            }
            else
            {
                LiveData.TransactionInfoList.Add(new TransactionInfo(transactionId, version));
            }
        }

        public string RetrieveTransactionVersion(string transactionId)
        {
            var tra = LiveData.TransactionInfoList.FirstOrDefault(t => t.TransactionId == transactionId);
            if (tra != null)
            {
                return string.Empty;
            }
            else
            {
                return tra.TransactionId;
            }
        }

        public void RecordSuccessfulRefund(Transaction transaction)
        {
            LiveData.TransactionList.Add(transaction);
        }
    }
}
