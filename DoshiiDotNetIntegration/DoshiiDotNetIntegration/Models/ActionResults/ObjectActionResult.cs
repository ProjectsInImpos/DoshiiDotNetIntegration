using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class ObjectActionResult<T>:ActionResultBasic
    {
        public T ReturnObject { get; set; }
    }
}
