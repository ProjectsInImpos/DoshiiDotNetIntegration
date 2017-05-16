﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// Surcharges and Discounts that are applied at an Order level.
    /// This model should not be used to record Product level discounts - discounts applied at a product level should be applied directly to the price attached to the product itself. 
    /// Surcharges should have a positive price.
    /// Discounts should have a negative price. 
    /// </summary>
    public class Surcount : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Surcount()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
		    Type = String.Empty;
		    Id = String.Empty;
		    Value = 0;
			Amount = 0;
		    RewardId = string.Empty;
		}

        /// <summary>
        /// The Name of the surcount
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Amount / value of the surcount in cents. 
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// the value of the surcount
        /// if the <see cref="Type"/> = 'absolute' this value will be equal to the amount that should be applied to the reward. 
        /// if the <see cref="Type"/> = 'percentage' this is the percent that needs to be used to calculate the value of the report. 
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// The type of the surcount ('absolute' or 'percentage')
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The posId for the surcount
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The reward Id if the surcount was applied sue to a reward. 
        /// </summary>
        public string RewardId { get; set; }

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion

        protected bool Equals(Surcount other)
        {
            return string.Equals(Name, other.Name) && Amount == other.Amount && Value == other.Value && string.Equals(Type, other.Type) && string.Equals(Id, other.Id) && string.Equals(RewardId, other.RewardId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Surcount) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Amount.GetHashCode();
                hashCode = (hashCode*397) ^ Value.GetHashCode();
                hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (RewardId != null ? RewardId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
