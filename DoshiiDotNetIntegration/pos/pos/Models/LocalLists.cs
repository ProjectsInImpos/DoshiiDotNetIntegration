using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Models
{
    public static class LocalLists
    {
        public static List<String> ProductTypes = new List<string>() {"none","single", "bundle"};

        public static List<String> SurcountTypes = new List<string>() { "absolute", "percentage" };

        public static List<String> OrderStatus = new List<string>() { "accepted", "pending", "rejected", "complete", "cancelled", "venue_cancelled" };

        public static List<String> OrderTypes = new List<string>() { "dinein", "delivery", "pickup" };

        public static List<String> BookingStatus = new List<string>() { "pending", "rejected", "accepted", "acknowledged", "cancelled", "cust_cancelled"};
    }
}
