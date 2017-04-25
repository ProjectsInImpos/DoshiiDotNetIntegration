using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    public class Employee : BaseCreatedAt, ICloneable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PosRef { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
        public string Id { get; set; }
        public string OrginistaionId { get; set; }

        protected bool Equals(Employee other)
        {
            return string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && string.Equals(Email, other.Email) && string.Equals(PosRef, other.PosRef) && string.Equals(Phone, other.Phone) && Equals(Address, other.Address) && string.Equals(Id, other.Id) && string.Equals(OrginistaionId, other.OrginistaionId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Employee) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PosRef != null ? PosRef.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (OrginistaionId != null ? OrginistaionId.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected void Clear()
        {
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.Email = string.Empty;
            this.PosRef = string.Empty;
            this.Phone= string.Empty;
            this.Address = new Address();
            this.Id = string.Empty;
            this.OrginistaionId = string.Empty;
        }

        public object Clone()
        {
            return (Employee)this.MemberwiseClone();
        }
    }
}
