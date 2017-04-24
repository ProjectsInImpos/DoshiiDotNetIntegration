using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class Log
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
	    public string EmployeePosRef { get; set; }
	    public string DeviceRef { get; set; }
	    public string DeviceName { get; set; }
	    public string Area { get; set; }

        public string LogId { get; set; }
	    public string AppId { get; set; }
	    public string AppName { get; set; }
	    public string Action { get; set; }
        public string PerformedAt { get; set; }
    }
}
