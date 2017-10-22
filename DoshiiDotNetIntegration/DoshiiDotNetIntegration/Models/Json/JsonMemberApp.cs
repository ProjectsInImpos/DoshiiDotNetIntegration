using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    [DataContract]
    [Serializable]
    internal class JsonMemberApp
    {
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public JsonAddress Address { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "points")]
        public string Points { get; set; }
    }
}
