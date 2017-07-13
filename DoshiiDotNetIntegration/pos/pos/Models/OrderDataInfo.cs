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
    public class OrderDataInfo
    {
        public OrderDataInfo()
        {
        }

        public OrderDataInfo(string orderId, string version)
        {
            OrderId = orderId;
            Version = version;
        }

        public OrderDataInfo(string orderId, string version, string checkin)
        {
            OrderId = orderId;
            Version = version;
            Checkin = checkin;
        }

        [DataMember]
        [JsonProperty(PropertyName = "OrderId")]
        public string OrderId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "Checkin")]
        public string Checkin { get; set; }
    }
}
