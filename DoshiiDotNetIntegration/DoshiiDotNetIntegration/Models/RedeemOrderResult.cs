using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class RedeemOrderResult : ICloneable
    {
        /// <summary>
        /// the Order will be Null if the request to redeem the Order was not successfull. 
        /// </summary>
        Order order;

        protected bool Equals(RedeemOrderResult other)
        {
            return Equals(order, other.order);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RedeemOrderResult) obj);
        }

        public override int GetHashCode()
        {
            return (order != null ? order.GetHashCode() : 0);
        }

        public void Clear()
        {
            order = new Order();
        }

        public object Clone()
        {
            return (RedeemOrderResult)this.MemberwiseClone();
        }
    }
}
