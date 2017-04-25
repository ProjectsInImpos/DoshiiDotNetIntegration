using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a Doshii reward
    /// </summary>
    public class Reward : ICloneable
    {
        /// <summary>
        /// the Id of the reward
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the reward
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the description of the reward
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the Type of the reward 'absolute' or 'percentage'
        /// </summary>
        public string SurcountType { get; set; }

        /// <summary>
        /// the amount the reward is worth, 
        /// if the <see cref="SurcountType"/> = 'absolute' the SurcountAmount is the value of the reward. 
        /// if the <see cref="SurcountType"/> = 'percentage' the SurcountAmount is the percentage value of the reward.  
        /// </summary>
        public decimal SurcountAmount { get; set; }
        
        /// <summary>
        /// the name of the <see cref="App"/> associated with the reward. 
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// the last time the reward was updated. 
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// the time the reward was created. 
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// the URI to retreive details about the reward from Doshii. 
        /// </summary>
        public Uri Uri { get; set; }

        protected bool Equals(Reward other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(Description, other.Description) && string.Equals(SurcountType, other.SurcountType) && SurcountAmount == other.SurcountAmount && string.Equals(AppName, other.AppName) && UpdatedAt.Equals(other.UpdatedAt) && CreatedAt.Equals(other.CreatedAt) && Equals(Uri, other.Uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Reward) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (SurcountType != null ? SurcountType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ SurcountAmount.GetHashCode();
                hashCode = (hashCode*397) ^ (AppName != null ? AppName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ UpdatedAt.GetHashCode();
                hashCode = (hashCode*397) ^ CreatedAt.GetHashCode();
                hashCode = (hashCode*397) ^ (Uri != null ? Uri.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected void Clear()
        {
            this.Id = string.Empty;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.SurcountType = string.Empty;
            this.SurcountAmount = 0;
            this.AppName = string.Empty;
            this.UpdatedAt = null;
            this.CreatedAt = null;
            this.Uri = new Uri("");
        }

        public object Clone()
        {
            return (Reward)this.MemberwiseClone();
        }
    }
}
