using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pos.Models
{
    [DataContract]
    [Serializable]
    public class TransactionInfo
    {
        public TransactionInfo(string transactionId, string version)
        {
            TransactionId = transactionId;
            Version = version;
        }

        [DataMember]
        [JsonProperty(PropertyName = "TransactionId")]
        public string TransactionId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }
    }
}
