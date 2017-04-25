using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class RejectionCode
    {
        /// <summary>
        /// the rejection code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// description of the rejection code
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the API entity the rejection code is associated with. 
        /// </summary>
        public string Entity { get; set; }
    }
}
