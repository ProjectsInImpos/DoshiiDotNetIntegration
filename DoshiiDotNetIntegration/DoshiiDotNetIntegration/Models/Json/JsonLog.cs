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
    internal class JsonLog : JsonSerializationBase<JsonLog>
    {
        [DataMember]
        [JsonProperty(PropertyName = "employeeId")]
        public string EmployeeId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "employeeName")]
        public string EmployeeName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "employeePosRef")]
        public string EmployeePosRef { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "deviceRef")]
        public string DeviceRef { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "deviceName")]
        public string DeviceName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "area")]
        public string Area { get; set; }


        [DataMember]
        [JsonProperty(PropertyName = "logId")]
        public string LogId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "appId")]
        public string AppId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "appName")]
        public string AppName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "performedAt")]
        public string PerformedAt { get; set; }

        #region serializeMembers

        public bool ShouldSerializeLogId()
        {
            return false;
        }
        public bool ShouldSerializeAppId()
        {
            return false;
        }
        public bool ShouldSerializeAppName()
        {
            return false;
        }
        public bool ShouldSerializeAction()
        {
            return false;
        }
        public bool ShouldSerializePerformedAt()
        {
            return false;
        }

        public bool ShouldSerializeEmployeeId()
        {
            return (!string.IsNullOrEmpty(EmployeeId));
        }

        public bool ShouldSerializeEmployeeName()
        {
            return (!string.IsNullOrEmpty(EmployeeName));
        }

        public bool ShouldSerializeEmployeePosRef()
        {
            return (!string.IsNullOrEmpty(EmployeePosRef));
        }
        public bool ShouldSerializeDeviceRef()
        {
            return (!string.IsNullOrEmpty(DeviceRef));
        }

        public bool ShouldSerializeDeviceName()
        {
            return (!string.IsNullOrEmpty(DeviceName));
        }

        public bool ShouldSerializeArea()
        {
            return (!string.IsNullOrEmpty(Area));
        }
        #endregion

    }
}
