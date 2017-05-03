using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The doshii representation of a product 
    /// A product is the highest level line item on orders.
    /// </summary>
    public class Product : ICloneable
    {
        /// <summary>
        /// The name of the product.
        /// This name will be displayed to Doshii users on the mobile app.
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// A description of the product that will be displayed on the mobile app
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The total price of the line item before surcounts are included qty * unit price. 
		/// </summary>
		public decimal TotalBeforeSurcounts { get; set; }

        /// <summary>
        /// The total price of the line item after surcounts are included qty * unit price + surcount. 
        /// </summary>
        public decimal TotalAfterSurcounts { get; set; }

        /// <summary>
        /// the unit price of the item 
        /// </summary>
        public decimal UnitPrice { get; set; }

        public string Uuid { get; set; }

        private List<string> _Tags;
        
        /// <summary>
        /// A list of the groups the product should be displayed under in the doshii mobile app
        /// This field is optional,
        /// Products can be added manually to groups in the doshii dashboard,
        /// If this list is populated the product will be automatically added to the groups included, this will reduce setup time. 
        /// </summary>
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
                _Tags = value.ToList<string>();
            }
        }

        private List<ProductOptions> _ProductOptions;
        
        /// <summary>
        /// A list of ProductOptions the customer can choose from to modify the product they are ordering.
        /// </summary>
        public IEnumerable<ProductOptions> ProductOptions {
            get
            {
                if (_ProductOptions == null)
                {
                    _ProductOptions = new List<ProductOptions>();
                }
                return _ProductOptions;
            } 
            set
            {
                _ProductOptions = value.ToList<ProductOptions>();
            }
        }

        private List<Surcount> _ProductSurcounts;

        /// <summary>
        /// A list of Surcharges available / selected for this product on the product.
        /// </summary>
        public IEnumerable<Surcount> ProductSurcounts
        {
            get
            {
                if (_ProductSurcounts == null)
                {
                    _ProductSurcounts = new List<Surcount>();
                }
                return _ProductSurcounts;
            }
            set
            {
                _ProductSurcounts = value.ToList<Surcount>();
            }
        }

		/// <summary>
		/// The POS Id of the product
		/// </summary>
		public string PosId { get; set; }

        /// <summary>
        /// The status of the item that is being ordered. 
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// the following are valid strings for type 'single' - a regular item, 'bundle' - an item with sub products.  
        /// </summary>
        public string Type { get; set; }

        private List<Product> _includedItems;

        /// <summary>
        /// A list of Surcharges available / selected for this product on the product.
        /// </summary>
        public List<Product> IncludedItems
        {
            get
            {
                if (_includedItems == null)
                {
                    _includedItems = new List<Product>();
                }
                return _includedItems;
            }
            set
            {
                _includedItems = value.ToList<Product>();
            }
        }


        private List<string> _menuDir;

        /// <summary>
        /// A list of Surcharges available / selected for this product on the product.
        /// </summary>
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
                _menuDir = value.ToList<string>();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Product()
		{
			_Tags = new List<string>();
			_ProductOptions = new List<ProductOptions>();
		    _menuDir = new List<string>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
			Description = String.Empty;
			TotalBeforeSurcounts = 0.0M;
		    TotalAfterSurcounts = 0.0M;
		    UnitPrice = 0.0M;
			_Tags.Clear();
			_ProductOptions.Clear();
			PosId = String.Empty;
		    Uuid = string.Empty;
            _includedItems.Clear();
            Quantity = 0;
		    Type = string.Empty;
		}

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			var product = (Product)this.MemberwiseClone();

			var tags = new List<string>();
		    foreach (string tag in this.Tags)
		    {
                tags.Add(tag);
		    }
			product.Tags = tags;

			var options = new List<ProductOptions>();
		    foreach (var option in this.ProductOptions)
		    {
                options.Add((ProductOptions)option.Clone());
		    }
			product.ProductOptions = options;

		    var includeItems = new List<Product>();
		    foreach (var pro in this.IncludedItems)
		    {
		        includeItems.Add((Product)pro.Clone());
		    }
		    product.IncludedItems = includeItems;

			return product;
		}

		#endregion

        protected bool Equals(Product other)
        {
            return Equals(_Tags, other._Tags) && Equals(_ProductOptions, other._ProductOptions) && Equals(_ProductSurcounts, other._ProductSurcounts) && Equals(_includedItems, other._includedItems) && Equals(_menuDir, other._menuDir) && string.Equals(Name, other.Name) && string.Equals(Description, other.Description) && TotalBeforeSurcounts == other.TotalBeforeSurcounts && TotalAfterSurcounts == other.TotalAfterSurcounts && UnitPrice == other.UnitPrice && string.Equals(Uuid, other.Uuid) && string.Equals(PosId, other.PosId) && Quantity == other.Quantity && string.Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_Tags != null ? _Tags.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_ProductOptions != null ? _ProductOptions.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_ProductSurcounts != null ? _ProductSurcounts.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_includedItems != null ? _includedItems.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_menuDir != null ? _menuDir.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ TotalBeforeSurcounts.GetHashCode();
                hashCode = (hashCode*397) ^ TotalAfterSurcounts.GetHashCode();
                hashCode = (hashCode*397) ^ UnitPrice.GetHashCode();
                hashCode = (hashCode*397) ^ (Uuid != null ? Uuid.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PosId != null ? PosId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Quantity.GetHashCode();
                hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
