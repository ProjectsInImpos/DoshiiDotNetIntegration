using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
	/// <summary>
	/// An object representing a doshii booking
	/// </summary>
    public class Booking : BaseCreatedAt, ICloneable
	{
		/// <summary>
		/// the Id of the booking
		/// </summary>
        public String Id { get; set; }

	    private List<string> _tableNames;

	    /// <summary>
	    /// the table names associated with the booking. 
	    /// </summary>
	    public List<string> TableNames
	    {
	        get
	        {
	            if (_tableNames == null)
	            {
	                _tableNames = new List<string>();
	            }
                return _tableNames;
	        }
	        set { _tableNames = value; }
	    }

        /// <summary>
        /// the dateTime of the booking. 
        /// </summary>
		public DateTime? Date { get; set; }

        /// <summary>
        /// the amount of covers associated with the booking. 
        /// </summary>
		public int Covers { get; set; }

        /// <summary>
        /// the <see cref="Consumer"/> associated with the booking. 
        /// </summary>
		public Consumer Consumer { get; set; }
        
        public String CheckinId { get; set; }
        
        public String Status { get; set; }

        /// <summary>
        /// the <see cref="App"/> associated with the booking. 
        /// </summary>
		public App App { get; set; }

        protected void Clear()
        {
            this.Id = string.Empty;
            this.TableNames.Clear();
            this.Date = null;
            this.Covers = 0;
            this.Consumer = new Consumer();
            this.CheckinId = string.Empty;
            this.App = new App();
            this.Status = string.Empty;
        }
        
        protected bool Equals(Booking other)
	    {
	        return string.Equals(Id, other.Id) && Equals(TableNames, other.TableNames) && Date.Equals(other.Date) && Covers == other.Covers && Equals(Consumer, other.Consumer) && string.Equals(CheckinId, other.CheckinId) && string.Equals(App, other.App);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((Booking) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = (Id != null ? Id.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (TableNames != null ? TableNames.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ Date.GetHashCode();
	            hashCode = (hashCode*397) ^ Covers;
	            hashCode = (hashCode*397) ^ (Consumer != null ? Consumer.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (CheckinId != null ? CheckinId.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (App != null ? App.GetHashCode() : 0);
	            return hashCode;
	        }
	    }

	    public object Clone()
	    {
            var booking = (Booking)this.MemberwiseClone();
            var tnames = new List<string>();
            foreach (var tname in this.TableNames)
            {
                tnames.Add((string)tname.Clone());
            }
            booking.TableNames = tnames;

            return booking;
	    }

	}
}
