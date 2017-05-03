using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// the class representing a member in Doshii
    /// </summary>
    public class Member : BaseCreatedAt, ICloneable
    {
        /// <summary>
        /// constructor. 
        /// </summary>
        public Member()
        {
            Address = new Address();
        }
        
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

        private List<App> _Apps;

        /// <summary>
        /// a list of <see cref="App"/> associated with the member
        /// </summary>
        public IEnumerable<App> Apps
        {
            get
            {
                if (_Apps == null)
                {
                    _Apps = new List<App>();
                }
                return _Apps;
            }
            set
            {
                _Apps = value.ToList<App>();
            }
        }

        protected bool Equals(Member other)
        {
            return Equals(_Apps, other._Apps) && string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && string.Equals(Email, other.Email) && string.Equals(Phone, other.Phone) && Equals(Address, other.Address) && string.Equals(Ref, other.Ref);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Member) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_Apps != null ? _Apps.GetHashCode() : 0);
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
            this._Apps.Clear();
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
            var returnMember = (Member)this.MemberwiseClone();
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

