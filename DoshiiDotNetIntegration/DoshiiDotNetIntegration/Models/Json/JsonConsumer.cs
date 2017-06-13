using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DoshiiDotNetIntegration.Models.Json.JsonBase;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A doshii consumer
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonConsumer : JsonSerializationBase<JsonConsumer>
    {
        /// <summary>
        /// the url for the consumers photo
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "photoURL")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// is this an anonymous user. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "anonymous")]
        public bool Anonymous { get; set; }
        
        /// <summary>
        /// The consumers name
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// the consumers phone number
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public JsonAddress Address { get; set; }

        /// <summary>
        /// Notes specific to this Order, 
        /// this may include:
        /// Notes about delivery location,
        /// Notes about allergies,
        /// Notes about a booking that has been made,
        /// Notes about special requests for the delivery. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        #region Serialize methods

        public bool ShouldSerializeFirstName()
        {
            return (!string.IsNullOrEmpty(FirstName));
        }

        public bool ShouldSerializeLastName()
        {
            return (!string.IsNullOrEmpty(LastName));
        }

        public bool ShouldSerializeAddress()
        {
            return (this.Address != null);
        }

        public bool ShouldSerializePhotoUrl()
        {
            return false;
        }

        public bool ShouldSerializeAnonymous()
        {
            return false;
        }

        public bool ShouldSerializeNotes()
        {
            return (!string.IsNullOrEmpty(Notes));
        }

        public bool ShouldSerializeEmail()
        {
            return (!string.IsNullOrEmpty(Email));
        }
        

        
        #endregion
    }
}
