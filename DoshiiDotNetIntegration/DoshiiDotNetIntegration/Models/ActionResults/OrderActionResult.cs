﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class OrderActionResult : ActionResultBasic
    {
        public Order Order { get; set; }
        public string OrderId { get; set; }
    }
}
