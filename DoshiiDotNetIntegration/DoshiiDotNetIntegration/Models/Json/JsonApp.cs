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

        private List<string> _types;

        [DataMember]
        [JsonProperty(PropertyName = "types")]
        public List<string> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new List<string>();
                }
                return _types;
            }
            set { _types = value; }
        }

        [DataMember]
        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "appMember")]
        public JsonMemberApp AppMember { get; set; }
    }
}
