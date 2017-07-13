using System;
using System.Collections.Generic;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// an object representing a checkin
    /// </summary>
    public class Checkin : BaseCreatedAt, ICloneable
    {
        /// <summary>
        /// the Id of the checkin. 
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// checkin id in the POS system
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// the table names associated with the checkin
        /// </summary>
        public List<string> TableNames { get; set; }

        /// <summary>
        /// the covers associated with the checkin
        /// </summary>
        public int Covers { get; set; }

        /// <summary>
        /// the <see cref="Consumer"/> associated with the checkin
        /// </summary>
        public Consumer Consumer { get; set; }

        /// <summary>
        /// the time the checkin was completed. 
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Associated booking
        /// </summary>
        public string BookingId { get; set; }


        protected bool Equals(Checkin other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Ref, other.Ref) &&
                   Equals(TableNames, other.TableNames) && Covers == other.Covers && Equals(Consumer, other.Consumer) &&
                   CompletedAt.Equals(other.CompletedAt) && Equals(BookingId, other.BookingId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Checkin) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Ref != null ? Ref.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TableNames != null ? TableNames.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Covers;
                hashCode = (hashCode*397) ^ (Consumer != null ? Consumer.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CompletedAt.GetHashCode();
                hashCode = (hashCode * 397) ^ (BookingId != null ? BookingId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            var checkin = (Checkin)this.MemberwiseClone();
            var tableNames = new List<string>();
            foreach (var table in this.TableNames)
            {
                tableNames.Add((string)table.Clone());
            }
            checkin.TableNames = tableNames;

            return checkin;
        }

        protected void Clear()
        {
            this.Id = string.Empty;
            this.Ref = String.Empty;
            this.TableNames.Clear();
            this.Covers = 0;
            this.Consumer = new Consumer();
            this.CompletedAt = null;
            this.BookingId = string.Empty;
        }
    }
}
