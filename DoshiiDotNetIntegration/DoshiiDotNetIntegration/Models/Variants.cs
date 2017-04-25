using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// Variants are available to modify products on the Doshii app,
    /// Examples of variants may include;
    /// Steak cooking methods eg 'rare, medium, or well done',
    /// sauces eg 'Tomato, bbq, sour cream'
    /// or sides eg 'chips, veg, salad'
    /// each variant can have a price attached to it.
    /// </summary>
    public class Variants : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Variants()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
			Price = 0.0M;
			PosId = String.Empty;
			//SelectedOptionalVariant = false;
		}

        /// <summary>
        /// The name of the variant that will be displayed on the mobile app
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The price of the variant in cents
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The POS Id of the variant.
        /// </summary>
        public string PosId { get; set; }

		/// <summary>
		/// This field will be true if the variant has been selected.
		/// </summary>
		//public bool SelectedOptionalVariant { get; set; }

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion

        protected bool Equals(Variants other)
        {
            return string.Equals(Name, other.Name) && Price == other.Price && string.Equals(PosId, other.PosId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Variants) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Price.GetHashCode();
                hashCode = (hashCode*397) ^ (PosId != null ? PosId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
