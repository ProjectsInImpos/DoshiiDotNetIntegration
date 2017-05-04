using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;
using DoshiiDotNetIntegration.Models.Json.JsonBase;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Base
{
    [DataContract]
    [Serializable]
    internal class DoshiiHttpErrorMessage : JsonSerializationBase<DoshiiHttpErrorMessage>
    {
        [DataMember]
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
