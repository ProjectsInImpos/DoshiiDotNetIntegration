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
    internal class JsonRejectionCode : JsonSerializationBase<JsonRejectionCode>
    {
        /// <summary>
        /// the rejection code
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// description of the rejection code
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// the API entity the rejection code is associated with. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "entity")]
        public string Entity { get; set; }
    }
}
