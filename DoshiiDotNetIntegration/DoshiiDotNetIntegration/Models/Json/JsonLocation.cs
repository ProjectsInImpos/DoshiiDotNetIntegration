﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json.JsonBase;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    [DataContract]
    [Serializable]
    internal class JsonLocation : JsonBaseCreatedAt<JsonLocation>
    {
        /// <summary>
        /// the DoshiiId for the venue - give this value to partners to allow them to send orders and payments to your venue. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        /// <summary>
        /// the name of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "addressLine1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "addressLine2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// the city element of the venue address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// the state element of the venue address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// the postal code of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "country")]
        public string PostalCode { get; set; }

        /// <summary>
        /// the country element of the venue address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "postalCode")]
        public string Country { get; set; }

        /// <summary>
        /// the phone number of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the last time the venue was disconnected - will be null if the venue is connected. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "disconnectedDate")]
        public DateTime? DisconnectedDate { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "vendorId")]
        public string VendorId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "organisationId")]
        public string OrganisationId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        #region serializeMembers

        public bool ShouldSerializeLatitude()
        {
            return !string.IsNullOrEmpty(Latitude);
        }

        public bool ShouldSerializeLongitude()
        {
            return !string.IsNullOrEmpty(Longitude);
        }
        
        public bool ShouldSerializeVendorId()
        {
            return false;
        }

        public bool ShouldSerializeOrganisationId()
        {
            return !string.IsNullOrEmpty(OrganisationId);
        }

        public bool ShouldSerializeToken()
        {
            return false;
        }

        public bool ShouldSerializeApiVersion()
        {
            return false;
        }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeDisconnectedDate()
        {
            return false;
        }

        
        #endregion
    }
}
