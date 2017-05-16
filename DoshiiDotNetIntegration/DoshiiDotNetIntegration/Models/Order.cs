using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// A Doshii Order with a counsumer
    /// </summary>
    public class Order : BaseStatus, ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
        public Order()
		{
			_surcounts = new List<Surcount>();
			_items = new List<Product>();
            _log = new List<Log>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
		    DoshiiId = String.Empty;
			Status = String.Empty;
			InvoiceId = String.Empty;
			CheckinId = String.Empty;
			LocationId = String.Empty;
			_surcounts.Clear();
			Version = String.Empty;
			Uri = null;
		    MemberId = string.Empty;
		    Phase = string.Empty;
			_items.Clear();
		    RequiredAt = null;
		    AvailableEta = null;
		    ManuallyAccepted = false;
            _log.Clear();
		    RejectionCode = string.Empty;
		    RejectionReason = string.Empty;
		    Consumer = null;
		}


        public Consumer Consumer { get; set; }
        
        /// <summary>
        /// Order id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Doshii Id for the Order, this is used to get unlinked orders
        /// </summary>
        public string DoshiiId { get; set; }

        /// <summary>
        /// type of Order 'delivery' or 'pickup'
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Unique identifier for the invoice once the Order is paid for.
        /// </summary>
        public string InvoiceId{ get; set; }


        public string MemberId { get; set; }

        /// <summary>
        /// Free text representation of where the Order is in the it's lifecycle at the venue. 
        /// </summary>
        public string Phase { get; set; }

        /// <summary>
        /// The CheckinId the Order is associated with, the doshii system uses this checkinId to relate tables to orders, to delete a table allocation you must have the
        /// Order checkIn Id. 
        /// </summary>
        public string CheckinId { get; set; }

		/// <summary>
		/// The Id of the location that the Order was created in.
		/// </summary>
		public string LocationId { get; set; }

		private List<Surcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and Order level
		/// Surcounts are discounts and surcharges / discounts should have a negative value. 
		/// </summary>
		public List<Surcount> Surcounts
		{
			get
			{
				if (_surcounts == null)
				{
					_surcounts = new List<Surcount>();
				}
				return _surcounts;
			}
			set { _surcounts = value.ToList<Surcount>(); }
		}
        
        /// <summary>
		/// An obfuscated string representation of the version for the Order in Doshii.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
        /// the time the Order is required if it is required in the future, 
        /// string will be empty is it is required now. 
        /// </summary>
        public DateTime? RequiredAt { get; set; }

        public DateTime? AvailableEta { get; set; }

        private List<Product> _items;
        
        /// <summary>
        /// A list of all the items included in the Order. 
        /// </summary>
        public List<Product> Items {
            get
            {
                if (_items == null)
                {
                    _items = new List<Product>();
                }
                return _items;
            }
            set { _items = value.ToList<Product>(); } 
        }

        public string LogUri { get; set; }

        private List<Log> _log;

        /// <summary>
        /// A list of all the items included in the Order. 
        /// </summary>
        public List<Log> Log
        {
            get
            {
                if (_log == null)
                {
                    _log = new List<Log>();
                }
                return _log;
            }
            set { _log = value.ToList<Log>(); }
        }


        public bool ManuallyAccepted { get; set; }

        public string TransactionsUri { get; set; }

        public string RejectionCode { get; set; }

        public string RejectionReason { get; set; }

        /// <summary>
        /// The OrderCreatedByAppId will be populated by the sdk when orders are created or updated with the appId of the app that created or updated the Order for Order creted events or Order updated events. 
        /// </summary>
        public string OrderCreatedByAppId { get; set; }

        public string OrderLastUpdateByAppId { get; set; }

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the calling instance.</returns>
		public object Clone()
		{
			var order = (Order)this.MemberwiseClone();

			// Memberwise clone doesn't handle recursive cloning of internal properties such as lists
			// here I am overwriting the list with cloned copies of the list items
			var surcounts = new List<Surcount>();
			foreach (var surcount in this.Surcounts)
			{
				surcounts.Add((Surcount)surcount.Clone());
			}
			order.Surcounts = surcounts;
            return order;
		}

        protected bool Equals(Order other)
        {
            return Equals(_surcounts, other._surcounts) && Equals(_items, other._items) && Equals(_log, other._log) && Equals(Consumer, other.Consumer) && string.Equals(Id, other.Id) && string.Equals(DoshiiId, other.DoshiiId) && string.Equals(Type, other.Type) && string.Equals(InvoiceId, other.InvoiceId) && string.Equals(MemberId, other.MemberId) && string.Equals(Phase, other.Phase) && string.Equals(CheckinId, other.CheckinId) && string.Equals(LocationId, other.LocationId) && string.Equals(Version, other.Version) && RequiredAt.Equals(other.RequiredAt) && AvailableEta.Equals(other.AvailableEta) && ManuallyAccepted == other.ManuallyAccepted && Equals(TransactionsUri, other.TransactionsUri) && string.Equals(RejectionCode, other.RejectionCode) && string.Equals(RejectionReason, other.RejectionReason);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Order) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_surcounts != null ? _surcounts.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_items != null ? _items.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_log != null ? _log.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Consumer != null ? Consumer.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DoshiiId != null ? DoshiiId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (InvoiceId != null ? InvoiceId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MemberId != null ? MemberId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phase != null ? Phase.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CheckinId != null ? CheckinId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LocationId != null ? LocationId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ RequiredAt.GetHashCode();
                hashCode = (hashCode*397) ^ AvailableEta.GetHashCode();
                hashCode = (hashCode*397) ^ ManuallyAccepted.GetHashCode();
                hashCode = (hashCode*397) ^ (TransactionsUri != null ? TransactionsUri.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (RejectionCode != null ? RejectionCode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (RejectionReason != null ? RejectionReason.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
	}
}
