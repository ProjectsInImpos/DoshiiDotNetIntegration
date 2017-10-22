using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The address represents an address of a member in Doshii
    /// </summary>
    public class Address : ICloneable
    {
        /// <summary>
        /// Line 1 of the address
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Line 2 of the address
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// the Address city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// the address state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// the address post/zip code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// the address country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public Address()
        {
            Clear();
        }
        
        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            Line1 = string.Empty;
            Line2 = string.Empty;
            City = string.Empty;
            State = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
            
        }

        protected bool Equals(Address other)
        {
            return string.Equals(Line1, other.Line1) && string.Equals(Line2, other.Line2) &&
                   string.Equals(City, other.City) && string.Equals(State, other.State) &&
                   string.Equals(PostalCode, other.PostalCode) && string.Equals(Country, other.Country);

        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Line1 != null ? Line1.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Line2 != null ? Line2.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PostalCode != null ? PostalCode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(System.Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }

        public object Clone()
        {
            return (Address)this.MemberwiseClone();
        }
    }
}
