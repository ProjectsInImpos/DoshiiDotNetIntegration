using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Models
{
	/// <summary>
	/// This model is a container that represents the Order being placed with a table allocation.
	/// </summary>
	public class TableOrder : ICloneable
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TableOrder()
		{
			Table = new TableAllocation();
			Order = new Order();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Table.Clear();
			Order.Clear();
		}

		/// <summary>
		/// The table allocation for the Order.
		/// </summary>
		public TableAllocation Table
		{
			get;
			set;
		}

		/// <summary>
		/// The details of the Order.
		/// </summary>
		public Order Order
		{
			get;
			set;
		}

	    protected bool Equals(TableOrder other)
	    {
	        return Equals(Table, other.Table) && Equals(Order, other.Order);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((TableOrder) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((Table != null ? Table.GetHashCode() : 0)*397) ^ (Order != null ? Order.GetHashCode() : 0);
	        }
	    }

	    public object Clone()
	    {
            return (TableOrder)this.MemberwiseClone();
	    }
	}
}
