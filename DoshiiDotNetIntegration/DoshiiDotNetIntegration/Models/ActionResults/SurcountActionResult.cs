using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class SurcountActionResult: ActionResultBasic
    {
        public Surcount Surcount { get; set; }
        public string SurcountId { get; set; }
        
    }
}
