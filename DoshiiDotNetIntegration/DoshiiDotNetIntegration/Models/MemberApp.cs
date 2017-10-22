using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    public class MemberApp : BaseMember, ICloneable
    {
        /// <summary>
        /// constructor. 
        /// </summary>
        public MemberApp()
        {}

        public int Points { get; set; }

        protected bool Equals(MemberApp other)
        {
            return Points == other.Points && string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && string.Equals(Email, other.Email) && string.Equals(Phone, other.Phone) && Equals(Address, other.Address) && string.Equals(Ref, other.Ref);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MemberApp)obj);
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

        protected void Clear()
        {
            this.Id = string.Empty;
            this.Name = string.Empty;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.Address = new Address();
            this.Ref = string.Empty;
            
        }

        public object Clone()
        {
            var returnMember = (MemberApp)this.MemberwiseClone();
            return returnMember;
        }

    }
}
