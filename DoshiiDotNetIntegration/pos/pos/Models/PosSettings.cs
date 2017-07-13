using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace pos.Models
{
    [DataContract]
    [Serializable]
    public class PosSettings
    {
        public PosSettings()
        {
        }

        private int _orderNumber;

        [DataMember]
        [JsonProperty(PropertyName = "OrderNumber")]
        public int OrderNumber {
            get { return _orderNumber++; }
            set { _orderNumber = value; } 
        }

        [DataMember]
        [JsonProperty(PropertyName = "ConsumerNumber")]
        public int ConsumerNumber { get; set; }


        [DataMember]
        [JsonProperty(PropertyName = "BaseUrl")]
        public string BaseUrl { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "socketUrl")]
        public string socketUrl { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "secretKey")]
        public string secretKey { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "doshiiVendor")]
        public string doshiiVendor { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "locationToken")]
        public string locationToken { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "socketTimeOutSec")]
        public int socketTimeOutSec { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "orgId")]
        public string OrganisationId { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "UseSocketConnection")]
        public bool UseSocketConnection { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "UseMembership")]
        public bool UseMembership { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "UseReservations")]
        public bool UseReservations { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "UseApps")]
        public bool UseApps { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "ConfirmAllOrders")]
        public bool ConfirmAllOrders { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "ConfirmAllTransactions")]
        public bool ConfirmAllTransactions { get; set; }
        [DataMember]
        [JsonProperty(PropertyName = "ConfirmAllRefunds")]
        public bool ConfirmAllRefunds { get; set; }
    }

}
