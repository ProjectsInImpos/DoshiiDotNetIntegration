using System;
using System.Collections.Generic;
using System.Linq;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// the class representing a member in Doshii
    /// </summary>
    public class MemberOrg : BaseMember, ICloneable
    {
        /// <summary>
        /// constructor. 
        /// </summary>
        public MemberOrg()
        {
            Address = new Address();
        }
        
        private List<App> _apps;

        /// <summary>
        /// a list of <see cref="App"/> associated with the member
        /// </summary>
        public IEnumerable<App> Apps
        {
            get
            {
                if (_apps == null)
                {
                    _apps = new List<App>();
                }
                return _apps;
            }
            set
            {
                _apps = value.ToList<App>();
            }
        }

        protected bool Equals(MemberOrg other)
        {
            if (other == null)
            {
                // If it is null then it is not equal to this instance.
                return false;
            }


            if (this.Id != other.Id)
            {
                return false;
            }

            if (this.Name != other.Name)
            {
                return false;
            }

            if (this.Email != other.Email)
            {
                return false;
            }

            if (this.Phone != other.Phone)
            {
                return false;
            }

            if (this.Address != null && other.Address != null)
            {
                if (!this.Address.Equals(other.Address))
                {
                    return false;
                }
            }
            else if ((this.Address == null && other.Address != null) ||
                     (this.Address != null && other.Address == null))
            {
                return false;
            }

           
            if (this.Ref != other.Ref)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MemberOrg) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_apps != null ? _apps.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Ref != null ? Ref.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Apps != null ? Apps.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected void Clear()
        {
            _apps.Clear();
            Id = string.Empty;
            Name = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = new Address();
            Ref = string.Empty;

        }

        public object Clone()
        {
            var returnMember = (MemberOrg)this.MemberwiseClone();
            var apps = new List<App>();
            foreach (var app in this.Apps)
            {
                apps.Add((App)app.Clone());
            }
            returnMember.Apps = apps;
            return returnMember;
        }
    }
}

