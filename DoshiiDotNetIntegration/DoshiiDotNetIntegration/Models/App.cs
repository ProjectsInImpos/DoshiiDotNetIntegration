using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// represents an app that a member is registered with. 
    /// </summary>
    public class App
    {
        public App()
        {
            _types = new List<string>();
        }

        /// <summary>
        /// the Id of the partner app
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the partner app
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the amount of points available for the member on this app
        /// </summary>
        public decimal Points { get; set; }

        private List<string> _types;

        public List<string> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new List<string>();
                }
                return _types;
            }
            set { _types = value.ToList<string>(); }
        }
    }
}
