using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// This class represents a location / venue in doshii
    /// </summary>
    public class Location : BaseCreatedAt, ICloneable
    {
        /// <summary>
        /// the DoshiiId for the venue - give this value to partners to allow them to send orders and payments to your venue. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the venue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// the city element of the venue address
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// the state element of the venue address
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// the postal code of the venue
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// the country element of the venue address
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// the phone number of the venue
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the last time the venue was disconnected - will be null if the venue is connected. 
        /// </summary>
        public DateTime? DisconnectedDate { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }


        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            Id = String.Empty;
            Name = String.Empty;
            AddressLine1 = String.Empty;
            AddressLine2 = String.Empty;
            City = String.Empty;
            State = String.Empty;
            PostalCode = String.Empty;
            Country = String.Empty;
            PhoneNumber = String.Empty;
            DisconnectedDate = null;
            CreatedAt = null;
            UpdatedAt = null;
            Uri = new Uri("");
        }

        protected bool Equals(Location other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(AddressLine1, other.AddressLine1) && string.Equals(AddressLine2, other.AddressLine2) && string.Equals(City, other.City) && string.Equals(State, other.State) && string.Equals(PostalCode, other.PostalCode) && string.Equals(Country, other.Country) && string.Equals(PhoneNumber, other.PhoneNumber) && DisconnectedDate.Equals(other.DisconnectedDate) && string.Equals(Latitude, other.Latitude) && string.Equals(Longitude, other.Longitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Location) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AddressLine1 != null ? AddressLine1.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AddressLine2 != null ? AddressLine2.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PostalCode != null ? PostalCode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ DisconnectedDate.GetHashCode();
                hashCode = (hashCode*397) ^ (Latitude != null ? Latitude.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Longitude != null ? Longitude.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return (Location)this.MemberwiseClone();
        }
    }
}
