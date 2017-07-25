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
    internal class JsonMenuProductIncludedItem : JsonMenuProduct
    {
        [DataMember]
        [JsonProperty(PropertyName = "quantity")]
        public string Quantity { get; set; }

        public override bool ShouldSerializeDescription()
        {
            return false;
        }

        public override bool ShouldSerializeMenuDir()
        {
            return false;
        }

        public override bool ShouldSerializeIncludedItems()
        {
            return false;
        }

        

        public override bool ShouldSerializeUuid()
        {
            return false;
        }

        public override bool ShouldSerializeType()
        {
            return false;
        }

        public override bool ShouldSerializeProductSurcounts()
        {
            return false;
        }

        public override bool ShouldSerializeTags()
        {
            return false;
        }
    }


}
