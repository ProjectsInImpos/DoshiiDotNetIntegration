using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json.JsonBase
{
    [DataContract]
    [Serializable]
    internal abstract class JsonBaseStatus<TSelf> : JsonBaseCreatedAt<TSelf>
    {
        /// <summary>
        /// status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        #region Serialize methods

        public bool ShouldSerializeStatus()
        {
            return (!string.IsNullOrEmpty(Status));
        }

        #endregion
    }
}
