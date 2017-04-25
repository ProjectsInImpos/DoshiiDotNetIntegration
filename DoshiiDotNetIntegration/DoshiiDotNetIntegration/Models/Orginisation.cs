using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class Orginisation : ICloneable
    {
        public string Name { get; set; }
	    public string Email { get; set; }
	    public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Abn { get; set; }
        public string CompanyNumber { get; set; }
        public Location Location { get; set; }

        protected bool Equals(Orginisation other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Email, other.Email) && string.Equals(Phone, other.Phone) && string.Equals(AddressLine1, other.AddressLine1) && string.Equals(AddressLine2, other.AddressLine2) && string.Equals(State, other.State) && string.Equals(City, other.City) && string.Equals(PostalCode, other.PostalCode) && string.Equals(Country, other.Country) && string.Equals(Abn, other.Abn) && string.Equals(CompanyNumber, other.CompanyNumber) && Equals(Location, other.Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Orginisation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AddressLine1 != null ? AddressLine1.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AddressLine2 != null ? AddressLine2.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PostalCode != null ? PostalCode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Abn != null ? Abn.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CompanyNumber != null ? CompanyNumber.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Location != null ? Location.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Clear()
        {
            Name = string.Empty;
	        Email = string.Empty;
	        Phone = string.Empty;
            AddressLine1 = string.Empty;
            AddressLine2 = string.Empty;
            State = string.Empty;
            City = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
            Abn = string.Empty;
            CompanyNumber = string.Empty;
            Location = new Location();
            
        }

        public object Clone()
        {
            return (Orginisation)this.MemberwiseClone();
        }
    }
}
