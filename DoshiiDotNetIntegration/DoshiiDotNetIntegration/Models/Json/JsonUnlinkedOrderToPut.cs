﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Json.JsonBase;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A Doshii order
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonUnlinkedOrderToPut : JsonBaseStatus<JsonUnlinkedOrderToPut>
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [DataMember]
        [JsonProperty(PropertyName = "phase")]
        public string Phase { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "memberId")]
        public string MemberId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "availableEta")]
        public DateTime? AvailableEta { get; set; }

        private List<JsonOrderSurcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and order level
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
		/// An obfuscated string representation of the version of the order in Doshii.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }

		private List<JsonOrderProduct> _items;
        
        /// <summary>
        /// A list of all the items included in the order. 
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
        [JsonProperty(PropertyName = "rejectionCode")]
        public string RejectionCode { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "rejectionReason")]
        public string RejectionReason { get; set; }

        #region serializeMembers

        public bool ShouldSerializeVersion()
        {
            return (!string.IsNullOrEmpty(Version));
        }

        public bool ShouldSerializeId()
        {
            return (!string.IsNullOrEmpty(Id));
        }

        public bool ShouldSerializePhase()
        {
            return (!string.IsNullOrEmpty(Phase));
        }

        public bool ShouldSerializeMemberId()
        {
            return (!string.IsNullOrEmpty(MemberId));
        }

        #endregion

        
    }
}
