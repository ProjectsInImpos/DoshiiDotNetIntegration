using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Rhino.Mocks.Constraints;

namespace DoshiiDotNetIntegration.Models.Json.JsonBase
{
    [DataContract]
    [Serializable]
    internal abstract class JsonBaseCreatedAt<TSelf> : JsonSerializationBase<TSelf>
    {
        /// <summary>
        /// Created At
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Updated At
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The URI of resource 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }

        #region Serialize methods

        public bool ShouldSerializeCreatedAt()
        {
            return false;
        }

        public bool ShouldSerializeUpdatedAt()
        {
            return false;
        }

        public bool ShouldSerializeUri()
        {
            return false;
        }

        #endregion
    }
}
