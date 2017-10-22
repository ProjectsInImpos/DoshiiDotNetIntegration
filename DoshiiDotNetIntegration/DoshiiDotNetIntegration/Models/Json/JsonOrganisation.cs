using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Json.JsonBase;

namespace DoshiiDotNetIntegration.Models.Json
{
    [DataContract]
    [Serializable]
    internal class JsonOrganisation : JsonSerializationBase<JsonOrganisation>
    {
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "addressLine1")]
        public string AddressLine1 { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "addressLine2")]
        public string AddressLine2 { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "abn")]
        public string Abn { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "companyNumber")]
        public string CompanyNumber { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "location")]
        public JsonLocation Location { get; set; }
    }
}
