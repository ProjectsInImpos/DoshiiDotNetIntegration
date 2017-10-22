using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using DoshiiDotNetIntegration.Models.Json.JsonBase;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// The doshii representation of a product 
    /// A product is the highest level line item on orders.
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonOrderProduct : JsonSerializationBase<JsonOrderProduct>
    {
        /// <summary>
        /// The quantity of the item ordered.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// The name of the product.
        /// This name will be displayed to Doshii users on the mobile app.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

		/// <summary>
		/// A description of the product that will be displayed on the mobile app
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		/// <summary>
		/// The Unitprice the product will be sold for through the mobile app, 
		/// This price is to be represented in cents. 
		/// </summary>
		[DataMember]
        [JsonProperty(PropertyName = "unitPrice")]
		public string UnitPrice { get; set; }

        /// <summary>
        /// The total price of the line item before surcounts are included qty * unit price. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "totalBeforeSurcounts")]
        public string TotalBeforeSurcounts { get; set; }

        /// <summary>
        /// The total price of the line item after surcounts are included qty * unit price + surcount. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "totalAfterSurcounts")]
        public string TotalAfterSurcounts { get; set; }

        private List<string> _Tags;
        
        /// <summary>
        /// A list of the groups the product should be displayed under in the doshii mobile app
        /// This field is optional,
        /// Products can be added manually to groups in the doshii dashboard,
        /// If this list is populated the product will be automatically added to the groups included, this will reduce setup time. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags 
        { 
            get
            { 
                if (_Tags == null)
                {
                    _Tags = new List<string>();
                }
                return _Tags;
            }
            set
            {
                _Tags = value;
            }
        }

        private List<JsonOrderProductOptions> _ProductOptions;
        
        /// <summary>
        /// A list of ProductOptions the customer can choose from to modify the product they are ordering.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "options")]
		public List<JsonOrderProductOptions> ProductOptions
		{
            get
            {
                if (_ProductOptions == null)
                {
					_ProductOptions = new List<JsonOrderProductOptions>();
                }
                return _ProductOptions;
            } 
            set
            {
                _ProductOptions = value;
            }
        }

        private List<JsonOrderSurcount> _ProductSurcounts;

        /// <summary>
        /// A list of surcounts that can / are applied to the product.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
        public List<JsonOrderSurcount> ProductSurcounts
        {
            get
            {
                if (_ProductSurcounts == null)
                {
                    _ProductSurcounts = new List<JsonOrderSurcount>();
                }
                return _ProductSurcounts;
            }
            set
            {
                _ProductSurcounts = value;
            }
        }

        [DataMember]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }


        private List<JsonOrderProductIncludedItems> _includedItems;

        /// <summary>
        /// A list of surcounts that can / are applied to the product.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "includedItems")]
        public List<JsonOrderProductIncludedItems> IncludedItems
        {
            get
            {
                if (_includedItems == null)
                {
                    _includedItems = new List<JsonOrderProductIncludedItems>();
                }
                return _includedItems;
            }
            set
            {
                _includedItems = value;
            }
        }

        [DataMember]
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }



		/// <summary>
		/// The POS Id of the product
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "posId")]
		public string PosId { get; set; }

        #region Serialize methods

        public virtual bool ShouldSerializePosId()
        {
            return (!string.IsNullOrEmpty(PosId));
        }

        public virtual bool ShouldSerializeDescription()
        {
            return (!string.IsNullOrEmpty(Description));
        }

        public virtual bool ShouldSerializeIncludedItems()
        {
            return IncludedItems.Any();
        }

        public virtual bool ShouldSerializeUuid()
        {
            return false;
        }
        ////////
        public virtual bool ShouldSerializeType()
        {
            return (!string.IsNullOrEmpty(Type));
        }

        public virtual bool ShouldSerializeProductSurcounts()
        {
            return true;
        }

        public virtual bool ShouldSerializeTags()
        {
            return true;
        }

        public virtual bool ShouldSerializeTotalAfterSurcounts()
        {
            return true;
        }

        public virtual bool ShouldSerializeTotalBeforeSurcounts()
        {
            return true;
        }
        #endregion

   }
}
