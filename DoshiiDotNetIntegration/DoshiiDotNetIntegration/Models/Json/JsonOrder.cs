using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Json.JsonBase;
using Rhino.Mocks.Constraints;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A Doshii Order
    /// </summary>
    [DataContract(Name = "Order")]
    [Serializable]
    internal class JsonOrder : JsonBaseStatus<JsonOrder>
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "consumer")]
        public JsonConsumer Consumer { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "doshiiId")]
        public string DoshiiId { get; set; }
        

        /// <summary>
        /// Order Type 'delivery' or 'pickup'
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        /// <summary>
        /// Unique identifier for the invoice once the Order is paid for.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "memberId")]
        public string MemberId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phase")]
        public string Phase { get; set; }

        /// <summary>
        /// The CheckinId the Order is associated with
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "CheckinId")]
        public string CheckinId { get; set; }

		/// <summary>
		/// The Id of the location that the Order was created in.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "locationId")]
		public string LocationId { get; set; }

		private List<JsonOrderSurcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and Order level
		/// Surcounts are discounts and surcharges / discounts should have a negative value. 
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "surcounts")]
		public List<JsonOrderSurcount> Surcounts
		{
			get
			{
				if (_surcounts == null)
				{
					_surcounts = new List<JsonOrderSurcount>();
				}
				return _surcounts;
			}
			set { _surcounts = value; }
		}
        
        /// <summary>
		/// An obfuscated string representation of the version of the Order in Doshii.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }

		

        /// <summary>
        /// the dateTime the Order is Required
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "requiredAt")]
        public DateTime? RequiredAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "manuallyAccepted")]
        public bool ManuallyAccepted { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "transactionsUri")]
        public string TransactionsUri { get; set; }
        
        /// <summary>
        /// the dateTime the Order is Required
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "availableEta")]
        public DateTime? AvailableEta { get; set; }

        private List<JsonOrderProduct> _items;
        
        /// <summary>
        /// A list of all the items included in the Order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
		public List<JsonOrderProduct> Items
		{
            get
            {
                if (_items == null)
                {
					_items = new List<JsonOrderProduct>();
                }
                return _items;
            }
			set { _items = value; } 
        }

        [DataMember]
        [JsonProperty(PropertyName = "log")]
        public string LogUri { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "rejectionCode")]
        public string RejectionCode { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "RejectionReason")]
        public string RejectionReason { get; set; }


        #region serialize methods

        public bool ShouldSerializeConsumer()
        {
            return (Consumer != null);
        }

        public bool ShouldSerializeRejectionCode()
        {
            return (!string.IsNullOrEmpty(RejectionCode));
        }

        public bool ShouldSerializeRejectionReason()
        {
            return (!string.IsNullOrEmpty(RejectionReason));
        }


        #endregion
    }
}
