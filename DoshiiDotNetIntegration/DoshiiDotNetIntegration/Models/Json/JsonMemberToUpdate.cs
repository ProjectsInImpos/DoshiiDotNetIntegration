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
    internal class JsonMemberToUpdate : JsonBaseStatus<JsonMemberToUpdate>
    {
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

        #region serializeMembers

        public bool ShouldSerializePhone()
        {
            return (!string.IsNullOrEmpty(Phone));
        }

        public bool ShouldSerializeEmail()
        {
            return (!string.IsNullOrEmpty(Email));
        }
        
        public bool ShouldSerializeRef()
        {
            return (!string.IsNullOrEmpty(Ref));
        }

        public bool ShouldSerializeFirstName()
        {
            return (!string.IsNullOrEmpty(FirstName));
        }

        public bool ShouldSerializeLastName()
        {
            return (!string.IsNullOrEmpty(LastName));
        }

        public bool ShouldSerializeName()
        {
            return (!string.IsNullOrEmpty(Name));
        }

        #endregion
    }
}
