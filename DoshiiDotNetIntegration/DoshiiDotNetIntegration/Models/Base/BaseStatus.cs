using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.Base
{
    public abstract class BaseStatus : BaseCreatedAt
    {
        public string Status { get; set; }
    }
}
