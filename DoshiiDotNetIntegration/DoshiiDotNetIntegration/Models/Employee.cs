using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Base;

namespace DoshiiDotNetIntegration.Models
{
    public class Employee : BaseCreatedAt
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PosRef { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
        public string Id { get; set; }
        public string OrginistaionId { get; set; }
        
    }
}
