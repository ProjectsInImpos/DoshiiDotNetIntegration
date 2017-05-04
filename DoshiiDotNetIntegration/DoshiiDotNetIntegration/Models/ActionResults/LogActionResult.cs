using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models.ActionResults
{
    public class LogActionResult: ActionResultBasic
    {
        private List<Log> _logList;

        public List<Log> LogList
        {
            get
            {
                if (_logList == null)
                {
                    _logList = new List<Log>();
                }
                return _logList;
            }
            set { _logList = value; }
        }
    }
}
