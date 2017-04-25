using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a doshii table
    /// </summary>
    public class Table : BaseCreatedAt, ICloneable
    {
        /// <summary>
        /// the name of the table. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the Max amount of covers that can be seated at the table. 
        /// </summary>
        public int Covers { get; set; }

        /// <summary>
        /// a flag indicating if the table is active in the pos
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// the <see cref="Criteria"/> associated with the table. 
        /// </summary>
        public TableCriteria Criteria { get; set; }

        protected bool Equals(Table other)
        {
            return string.Equals(Name, other.Name) && Covers == other.Covers && IsActive == other.IsActive && Equals(Criteria, other.Criteria);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Table) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Covers;
                hashCode = (hashCode*397) ^ IsActive.GetHashCode();
                hashCode = (hashCode*397) ^ (Criteria != null ? Criteria.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected void Clear()
        {
            this.Name = string.Empty;
            this.Covers = 0; 
            this.IsActive = false;
            this.Criteria = new TableCriteria();
        }

        public object Clone()
        {
            return (Table)this.MemberwiseClone();
        }
    }
}
