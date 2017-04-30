using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// an internal class used in the process of redeeming points. 
    /// </summary>
    internal class PointsRedeem: ICloneable
    {
        /// <summary>
        /// the orderId the points will be redeemed against. 
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// the amount of points to be redeemed. 
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// that <see cref="App.Id"/> representing the App the points will be redeemed against. 
        /// </summary>
        public string AppId { get; set; }

        protected bool Equals(PointsRedeem other)
        {
            return string.Equals(OrderId, other.OrderId) && Points == other.Points && string.Equals(AppId, other.AppId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PointsRedeem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (OrderId != null ? OrderId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Points;
                hashCode = (hashCode*397) ^ (AppId != null ? AppId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Clear()
        {
            OrderId = string.Empty;
            Points = 0;
            AppId = string.Empty;
        }

        public object Clone()
        {
            return (PointsRedeem)this.MemberwiseClone();
        }
    }
}
