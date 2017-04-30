using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// the criteria for a table in a venue. 
    /// </summary>
    public class TableCriteria : ICloneable
    {
        /// <summary>
        /// flag indicating if the table is communal. 
        /// </summary>
        public bool IsCommunal { get; set; }

        /// <summary>
        /// flag indicating if it is possible to merge the table with other tables. 
        /// </summary>
        public bool CanMerge { get; set; }

        /// <summary>
        /// flag indicating if the table allows smoking. 
        /// </summary>
        public bool IsSmoking { get; set; }

        /// <summary>
        /// flag indicating if the table is outdoors. 
        /// </summary>
        public bool IsOutdoor { get; set; }

        protected bool Equals(TableCriteria other)
        {
            return IsCommunal == other.IsCommunal && CanMerge == other.CanMerge && IsSmoking == other.IsSmoking && IsOutdoor == other.IsOutdoor;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TableCriteria) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsCommunal.GetHashCode();
                hashCode = (hashCode*397) ^ CanMerge.GetHashCode();
                hashCode = (hashCode*397) ^ IsSmoking.GetHashCode();
                hashCode = (hashCode*397) ^ IsOutdoor.GetHashCode();
                return hashCode;
            }
        }

        protected void Clear()
        {
            this.IsCommunal = false;
            this.CanMerge = false;
            this.IsSmoking = false;
            this.IsOutdoor = false;
        }

        public object Clone()
        {
            return (TableCriteria)this.MemberwiseClone();
        }
    }
}
