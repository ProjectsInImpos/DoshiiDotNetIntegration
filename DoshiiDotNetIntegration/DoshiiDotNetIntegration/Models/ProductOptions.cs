using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// Product options are lists of product variants that can be selected from to modify products.
    /// </summary>
    public class ProductOptions : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public ProductOptions()
		{
			_Variants = new List<Variant>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
			Min = 0;
			Max = 0;
			PosId = String.Empty;
			_Variants.Clear();
		}

        /// <summary>
        /// The name of this product options / or list of variants
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum amount of variants that must be chosen from this set of variants
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// The maximum amount of variants that can be chosen form this set of variants. 
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// The POS identifier for this set of variants. 
        /// </summary>
        public string PosId { get; set; }

        private List<Variant> _Variants;

        /// <summary>
        /// A List of Variant available to be selected from this list. 
        /// </summary>
        public List<Variant> Variants 
        {
            get
            {
                if (_Variants == null)
                {
                    _Variants = new List<Variant>();
                }
                return _Variants;
            }
            set
            {
                _Variants = value.ToList<Variant>();
            }
        } 
        
        #region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			var options = (ProductOptions)this.MemberwiseClone();

			var variants = new List<Variant>();
			foreach (var variant in this.Variants)
			{
				variants.Add((Variant)variant.Clone());
			}
			options.Variants = variants;

			return options;
		}

		#endregion

        protected bool Equals(ProductOptions other)
        {
            return Equals(_Variants, other._Variants) && string.Equals(Name, other.Name) && Min == other.Min && Max == other.Max && string.Equals(PosId, other.PosId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductOptions) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_Variants != null ? _Variants.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Min;
                hashCode = (hashCode*397) ^ Max;
                hashCode = (hashCode*397) ^ (PosId != null ? PosId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
