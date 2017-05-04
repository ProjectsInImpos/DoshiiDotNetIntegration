using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class EmployeeActionResult: ActionResultBasic
    {
        public Employee Employee { get; set; }
        public string EmployeeId { get; set; }
    }
}
