﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// A Doshii order
    /// </summary>
    public class Order : ICloneable
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
			Uri = String.Empty;
		    MemberId = string.Empty;
		    Phase = string.Empty;
			_items.Clear();
            _log.Clear();
		    RequiredAt = null;
		    ManuallyAccepted = false;
		    TransactionUri = string.Empty;
		    AvailableEta = null;
		}

        /// <summary>
        /// Order id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Doshii Id for the order, this is used to get unlinked orders
        /// </summary>
        public string DoshiiId { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// type of order 'delivery' or 'pickup'
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Unique identifier for the invoice once the order is paid for.
        /// </summary>
        public string InvoiceId{ get; set; }


        public string MemberId { get; set; }

        public bool ManuallyAccepted { get; set; }

        /// <summary>
        /// Free text representation of where the Order is in the it's lifecycle at the venue. 
        /// </summary>
        public string Phase { get; set; }

        /// <summary>
        /// The CheckinId the order is associated with, the doshii system uses this checkinId to relate tables to orders, to delete a table allocation you must have the
        /// order checkIn Id. 
        /// </summary>
        public string CheckinId { get; set; }

		/// <summary>
		/// The Id of the location that the order was created in.
		/// </summary>
		public string LocationId { get; set; }

        public DateTime? AvailableEta { get; set; }

		private List<Surcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and order level
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

        private List<Log> _log;

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
        
        /// <summary>
		/// An obfuscated string representation of the version for the order in Doshii.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// The URI of the order
		/// </summary>
		public string Uri { get; set; }

        public string TransactionUri { get; set; }

        /// <summary>
        /// the time the order is required if it is required in the future, 
        /// string will be empty is it is required now. 
        /// </summary>
        public DateTime? RequiredAt { get; set; }
        
        private List<Product> _items;
        
        /// <summary>
        /// A list of all the items included in the order. 
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
			var payments = new List<Transaction>();
			var surcounts = new List<Surcount>();
			foreach (var surcount in this.Surcounts)
			{
				surcounts.Add((Surcount)surcount.Clone());
			}
			order.Surcounts = surcounts;
            return order;
		}

		#endregion
	}
}
