using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class Orginisation
    {
      public string Name { get; set; }
	  public string Email { get; set; }
	  public string Phone { get; set; }      public string AddressLine1 { get; set; }      public string AddressLine2 { get; set; }      public string State { get; set; }      public string City { get; set; }      public string PostalCode { get; set; }      public string Country { get; set; }      public string Abn { get; set; }      public string CompanyNumber { get; set; }
      public Location Location { get; set; }
    }
}
