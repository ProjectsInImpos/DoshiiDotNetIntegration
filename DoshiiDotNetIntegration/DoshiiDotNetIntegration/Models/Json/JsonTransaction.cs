using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Json.JsonBase;

namespace DoshiiDotNetIntegration.Models.Json
{

    /// <summary>
    /// Payments that are made on the Doshii check
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonTransaction : JsonBaseStatus<JsonTransaction>
    {
        /// <summary>
        /// Unique number identifying this resource
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// identify the Order this transaction relates to
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// info about the payment
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        /// <summary>
        /// partner identifier for the transaction
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoice")]
        public string Invoice { get; set; }

        /// <summary>
        /// The amount that has been paid in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "amount")]
        public string PaymentAmount { get; set; }

        /// <summary>
        /// flag indicating if the pos will accept less than the total amount as a payment from the partner
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "acceptLess")]
        public bool AcceptLess { get; set; }

        /// <summary>
        /// flag indicating if the transaction was initiated by the partner
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "partnerInitiated")]
        public bool PartnerInitiated { get; set; }

        /// <summary>
        /// identifier for the partner that completed the transaction
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "createdByApp")]
        public string CreatedByApp { get; set; }

        /// <summary>
        /// An obfuscated string representation of the version of the Order in Doshii.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "tip")]
        public string Tip { get; set; }

        private List<string> _linkedTrxIds;

        /// <summary>
        /// A list of all surcounts applied at and Order level
        /// Surcounts are discounts and surcharges / discounts should have a negative value. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "linkedTrxIds")]
        public List<string> LinkedTrxIds 
        {
            get
            {
                if (_linkedTrxIds == null)
                {
                    _linkedTrxIds = new List<string>();
                }
                return _linkedTrxIds;
            }
            set { _linkedTrxIds = value; }
        }

        #region serialize methods

        public bool ShouldSerializeLinkedTrxIds()
        {
            return LinkedTrxIds.Count > 0;
        }
        
        public bool ShouldSerializeCreatedByApp()
        {
            return false;
        }

        public bool ShouldSerializeVersion()
        {
            return (!string.IsNullOrEmpty(Version));
        }

        public bool ShouldSerializeOrderId()
        {
            return (!string.IsNullOrEmpty(OrderId));
        }

        public bool ShouldSerializeReference()
        {
            return (!string.IsNullOrEmpty(Reference));
        }

        public bool ShouldSerializeUri()
        {
            return false;
        }

        public bool ShouldSerializePartnerInitiated()
        {
            return false;
        }

        public bool ShouldSerializeId()
        {
            return false;
            
        }

        public bool ShouldSerializeInvoice()
        {
            return (!string.IsNullOrEmpty(Invoice));
        }
        #endregion

        


    }
}
