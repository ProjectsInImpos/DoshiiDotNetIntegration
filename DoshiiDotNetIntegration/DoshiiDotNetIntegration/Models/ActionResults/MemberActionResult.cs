using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class MemberActionResult : ActionResultBasic
    {
        public Member Member { get; set; }
        public string MemberId { get; set; }
    }
}
