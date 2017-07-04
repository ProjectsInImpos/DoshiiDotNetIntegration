using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class Log : ICloneable
    {
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeName { get; set; }
	    public string EmployeePosRef { get; set; }
	    public string DeviceRef { get; set; }
	    public string DeviceName { get; set; }
	    public string Area { get; set; }

        public string LogId { get; set; }
	    public string AppId { get; set; }
	    public string AppName { get; set; }
	    public dynamic Action { get; set; }
        public string PerformedAt { get; set; }

        protected bool Equals(Log other)
        {
            return string.Equals(EmployeeId, other.EmployeeId) && string.Equals(EmployeeLastName, other.EmployeeLastName)&& string.Equals(EmployeeFirstName, other.EmployeeFirstName) && string.Equals(EmployeePosRef, other.EmployeePosRef) && string.Equals(DeviceRef, other.DeviceRef) && string.Equals(DeviceName, other.DeviceName) && string.Equals(Area, other.Area) && string.Equals(LogId, other.LogId) && string.Equals(AppId, other.AppId) && string.Equals(AppName, other.AppName) && string.Equals(Action, other.Action) && string.Equals(PerformedAt, other.PerformedAt);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Log) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EmployeeId != null ? EmployeeId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EmployeeFirstName != null ? EmployeeFirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EmployeeLastName != null ? EmployeeLastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EmployeePosRef != null ? EmployeePosRef.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DeviceRef != null ? DeviceRef.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DeviceName != null ? DeviceName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Area != null ? Area.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LogId != null ? LogId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AppId != null ? AppId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AppName != null ? AppName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Action != null ? Action.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PerformedAt != null ? PerformedAt.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Clear()
        {
            EmployeeId = string.Empty;
            EmployeeFirstName = string.Empty; 
	        EmployeePosRef = string.Empty;
	        DeviceRef = string.Empty;
	        DeviceName = string.Empty;
	        Area = string.Empty;

            LogId = string.Empty;
	        AppId = string.Empty;
	        AppName = string.Empty;
	        Action = string.Empty;
            PerformedAt = string.Empty;
        }

        public object Clone()
        {
            return (Log)this.MemberwiseClone();
        }
    }
}
