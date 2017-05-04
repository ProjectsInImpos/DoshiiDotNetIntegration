using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class CheckinActionResult : ActionResultBasic
    {
        public Checkin Checkin { get; set; }
        public string CheckinId { get; set; }
    }
}
