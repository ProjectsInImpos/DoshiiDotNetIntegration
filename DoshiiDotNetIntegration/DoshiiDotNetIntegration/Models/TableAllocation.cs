using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// A table allocation object
    /// </summary>
	public class TableAllocation : ICloneable
	{
		/// <summary>
		/// This is the CheckInId associated with the TableAllocation
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// This is the Name of the table the CheckIn is associated with.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The allocation status of the table. 
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// An obfuscated string representation of the table version in Doshii.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Constructor. 
		/// </summary>
		public TableAllocation()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
			Name = String.Empty;
			Status = String.Empty;
			Version = String.Empty;
		}

	    protected bool Equals(TableAllocation other)
	    {
	        return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(Status, other.Status) && string.Equals(Version, other.Version);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((TableAllocation) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = (Id != null ? Id.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Status != null ? Status.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
	            return hashCode;
	        }
	    }

	    public object Clone()
	    {
            return (TableAllocation)this.MemberwiseClone();
	    }
	}
}