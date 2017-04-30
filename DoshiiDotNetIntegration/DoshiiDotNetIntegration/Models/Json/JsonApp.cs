using System;
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
    internal class JsonApp : JsonSerializationBase<JsonApp>
    {

        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "points")]
        public string Points { get; set; }

        private List<string> _apps;

        [DataMember]
        [JsonProperty(PropertyName = "types")]
        public List<string> Surcounts
        {
            get
            {
                if (_apps == null)
                {
                    _apps = new List<string>();
                }
                return _apps;
            }
            set { _apps = value; }
        }

        [DataMember]
        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }
    }
}
