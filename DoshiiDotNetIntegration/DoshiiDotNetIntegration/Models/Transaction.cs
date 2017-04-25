using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{

    /// <summary>
    /// Transactions that are linked to orders in Doshii
    /// </summary>
    public class Transaction : BaseStatus, ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Transaction()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
            OrderId = String.Empty;
            Reference = String.Empty;
            Invoice = String.Empty;
		    PaymentAmount = 0.0M;
		    AcceptLess = false;
		    PartnerInitiated = false; 
            Partner = String.Empty;
		    Status = "pending";
            Version = String.Empty;
            Uri = new Uri("");
		    Tip = 0.0M;
		    CreatedAt = null;
		    UpdatedAt = null;
		}

        public decimal Tip { get; set; }

        /// <summary>
        /// Unique number identifying this resource
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// identify the order this transaction relates to
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// info about the payment
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// partner identifier for the transaction
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// The amount that has been paid in cents. 
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// flag indicating if the pos will accept less than the total amount as a payment from the partner
        /// </summary>
        public bool AcceptLess { get; set; }

        /// <summary>
        /// flag indicating if the transaction was initiated by the partner
        /// </summary>
        public bool PartnerInitiated { get; set; }

        /// <summary>
        /// identifier for the partner that completed the transaction
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// An obfuscated string representation of the version for the order in Doshii.
        /// </summary>
        public string Version { get; set; }

        #region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion

        protected bool Equals(Transaction other)
        {
            return Tip == other.Tip && string.Equals(Id, other.Id) && string.Equals(OrderId, other.OrderId) && string.Equals(Reference, other.Reference) && string.Equals(Invoice, other.Invoice) && PaymentAmount == other.PaymentAmount && AcceptLess == other.AcceptLess && PartnerInitiated == other.PartnerInitiated && string.Equals(Partner, other.Partner) && string.Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transaction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Tip.GetHashCode();
                hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (OrderId != null ? OrderId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Reference != null ? Reference.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Invoice != null ? Invoice.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ PaymentAmount.GetHashCode();
                hashCode = (hashCode*397) ^ AcceptLess.GetHashCode();
                hashCode = (hashCode*397) ^ PartnerInitiated.GetHashCode();
                hashCode = (hashCode*397) ^ (Partner != null ? Partner.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
