﻿using System;
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
    internal class JsonBooking : JsonBaseCreatedAt<JsonBooking>
    {
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "tableNames")]
        public List<string> TableNames { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "covers")]
        public string Covers { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "consumer")]
        public JsonConsumer Consumer { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "checkinId")]
        public String checkinId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "app")]
        public String App { get; set; }

        #region serializeMembers

        public bool ShouldSerializeApp()
        {
            return false;
        }

        #endregion serializeMembers
    }
}
