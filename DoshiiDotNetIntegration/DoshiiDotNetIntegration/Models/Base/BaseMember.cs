using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.Base
{
    public class BaseMember : BaseCreatedAt
    {
        /// <summary>
        /// the Id of the member
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the member
        /// </summary>
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// the email of the member
        /// </summary>
        public string Email { get; set; }

        protected bool Equals(BaseMember other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && string.Equals(Email, other.Email) && string.Equals(Phone, other.Phone) && Equals(Address, other.Address) && string.Equals(Ref, other.Ref);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseMember) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Ref != null ? Ref.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// the phone number of the member
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// the <see cref="Address"/> associated with the member
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// the Pos identifier for the member. 
        /// </summary>
        public string Ref { get; set; }
    }
}
