using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    [DataContract]
    [Serializable]
    internal class JsonEmployee
    {
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
        [JsonProperty(PropertyName = "posRef")]
        public string PosRef { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public JsonAddress Address { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "orginistaionId")]
        public string OrginistaionId { get; set; }
        
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }

        #region Serialize methods

        public bool ShouldSerializeFirstName()
        {
            return (!string.IsNullOrEmpty(FirstName));
        }

        public bool ShouldSerializeLastName()
        {
            return (!string.IsNullOrEmpty(LastName));
        }

        public bool ShouldSerializeEmail()
        {
            return (!string.IsNullOrEmpty(Email));
        }
        public bool ShouldSerializePosRef()
        {
            return (!string.IsNullOrEmpty(PosRef));
        }
        public bool ShouldSerializePhone()
        {
            return (!string.IsNullOrEmpty(Phone));
        }
        public bool ShouldSerializeAddress()
        {
            return (Address != null);
        }

        public bool ShouldSerializeId()
        {
            return (!string.IsNullOrEmpty(Id));
        }

        public bool ShouldSerializeOrginistaionId()
        {
            return (!string.IsNullOrEmpty(OrginistaionId));
        }

        public bool ShouldSerializeUpdatedAt()
        {
            return false;
        }

        public bool ShouldSerializeCreatedAt()
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
