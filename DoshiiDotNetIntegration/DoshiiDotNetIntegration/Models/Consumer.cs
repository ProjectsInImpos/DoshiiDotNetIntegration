using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The consumer model represents the individual customer in Doshii.
    /// </summary>
    public class Consumer : ICloneable
    {
        /// <summary>
		/// Constructor.
		/// </summary>
        public Consumer()
		{
			Clear();
		}
        
        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            Name = String.Empty;
            Phone = String.Empty;
            Address = null;
            Notes = String.Empty;
            PhotoUrl = string.Empty;
            Anonymous = false;
            FirstName = string.Empty;
            LastName = string.Empty;
        }


        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        /// <summary>
        /// the url for the consumers photo
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// is this an anonymous user. 
        /// </summary>
        public bool Anonymous { get; set; }
        
        /// <summary>
        /// The consumers name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the consumers phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// the email of the consumer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the <see cref="Address"/> associated with the consumer
        /// </summary>
        public Address Address { get; set; }
        
        /// <summary>
        /// Notes specific to this order, 
        /// this may include:
        /// Notes about delivery location,
        /// Notes about allergies,
        /// Notes about a booking that has been made,
        /// Notes about special requests for the delivery. 
        /// </summary>
        public string Notes { get; set; }

        protected bool Equals(Consumer other)
        {
            return string.Equals(PhotoUrl, other.PhotoUrl) && Anonymous == other.Anonymous && string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && string.Equals(Name, other.Name) && string.Equals(Phone, other.Phone) && string.Equals(Email, other.Email) && Equals(Address, other.Address) && string.Equals(Notes, other.Notes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Consumer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (PhotoUrl != null ? PhotoUrl.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Anonymous.GetHashCode();
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Notes != null ? Notes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return (Consumer)this.MemberwiseClone();
        }
    }
}
