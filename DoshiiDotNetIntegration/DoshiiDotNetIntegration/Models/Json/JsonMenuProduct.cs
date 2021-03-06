﻿using System;
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
    internal class JsonMenuProduct : JsonSerializationBase<JsonMenuProduct>
    {
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

        private List<JsonMenuProductOptions> _ProductOptions;
        
        /// <summary>
        /// A list of ProductOptions the customer can choose from to modify the product they are ordering.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "options")]
		public List<JsonMenuProductOptions> ProductOptions
		{
            get
            {
                if (_ProductOptions == null)
                {
					_ProductOptions = new List<JsonMenuProductOptions>();
                }
                return _ProductOptions;
            } 
            set
            {
                _ProductOptions = value;
            }
        }

        private List<JsonMenuSurcount> _ProductSurcounts;

        /// <summary>
        /// A list of surcounts that can / are applied to the product.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
        public List<JsonMenuSurcount> ProductSurcounts
        {
            get
            {
                if (_ProductSurcounts == null)
                {
                    _ProductSurcounts = new List<JsonMenuSurcount>();
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


        private List<string> _menuDir;

        /// <summary>
        /// A list of surcounts that can / are applied to the product.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "menuDir")]
        public List<string> MenuDir
        {
            get
            {
                if (_menuDir == null)
                {
                    _menuDir = new List<string>();
                }
                return _menuDir;
            }
            set
            {
                _menuDir = value;
            }
        }

        private List<JsonMenuProductIncludedItem> _includedItems;

        /// <summary>
        /// A list of surcounts that can / are applied to the product.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "includedItems")]
        public List<JsonMenuProductIncludedItem> IncludedItems
        {
            get
            {
                if (_includedItems == null)
                {
                    _includedItems = new List<JsonMenuProductIncludedItem>();
                }
                return _includedItems;
            }
            set
            {
                _includedItems = value;
            }
        }

        [DataMember]
        [JsonProperty(PropertyName = "Uuid")]
        public string Uuid { get; set; }


		/// <summary>
		/// The POS Id of the product
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "posId")]
		public string PosId { get; set; }

		/// <summary>
        /// The Unitprice the product will be sold for through the mobile app, 
        /// This price is to be represented in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "unitPrice")]
        public string UnitPrice { get; set; }


        #region Serialize methods

        public virtual bool ShouldSerializeDescription()
        {
            return !string.IsNullOrEmpty(Description);
        }

        public virtual bool ShouldSerializeMenuDir()
        {
            return MenuDir.Any();
        }

        public virtual bool ShouldSerializeIncludedItems()
        {
            return IncludedItems.Any();
        }

        public virtual bool ShouldSerializeUuid()
        {
            return false;
        }

        public virtual bool ShouldSerializeType()
        {
            return !string.IsNullOrEmpty(Type);
        }

        public virtual bool ShouldSerializeProductSurcounts()
        {
            return true;
        }

        public virtual bool ShouldSerializeTags()
        {
            return true;
        }
        
        
        
        
        
            
        
        #endregion

    }
}
