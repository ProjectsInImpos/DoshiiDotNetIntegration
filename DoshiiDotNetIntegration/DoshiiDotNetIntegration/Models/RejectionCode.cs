using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class RejectionCode : ICloneable
    {
        /// <summary>
        /// the rejection code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// description of the rejection code
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the API entity the rejection code is associated with. 
        /// </summary>
        public string Entity { get; set; }

        protected bool Equals(RejectionCode other)
        {
            return string.Equals(Code, other.Code) && string.Equals(Description, other.Description) && string.Equals(Entity, other.Entity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RejectionCode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Entity != null ? Entity.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected void Clear()
        {
            this.Code = string.Empty;
            this.Description = string.Empty;
            this.Entity = string.Empty;
        }

        public object Clone()
        {
            return (RejectionCode)this.MemberwiseClone();
        }
    }
}
