using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DoshiiDotNetIntegration.Models.Json.JsonBase;

namespace DoshiiDotNetIntegration.Models.Json
{
	/// <summary>
	/// The data transfer object used in internal communication between the SDK and the RESTful API
	/// for an Order containing a table allocation.
	/// </summary>
	[DataContract]
	[Serializable]
	internal class JsonTableOrder : JsonSerializationBase<JsonTableOrder>
	{
		/// <summary>
		/// The table allocation for the Order.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "table")]
        public JsonTableAllocationForCreate Table
		{
			get;
			set;
		}

		/// <summary>
		/// The Order details.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "Order")]
		public JsonOrderToPut JsonOrder
		{
			get;
			set;
		}
		
	}
}
