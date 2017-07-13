using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;
using pos.Models;

namespace pos
{
    public static class LiveData
    {
        public static List<App> AppsList;

        public static List<Booking> BookingList;

        public static List<Consumer> ConsumerList;

        public static List<Product> ProductList;

        public static List<Surcount> SurcountList;

        public static List<MemberOrg> MemberList;

        public static List<Order> OrdersList;

        public static List<Table> TablesList;

        public static List<Checkin> ChecinList;

        public static PosSettings PosSettings;

        public static List<OrderDataInfo> OrderDataList;

        public static List<Transaction> TransactionList;

        public static List<TransactionInfo> TransactionInfoList;

        public static List<Location> LocationList;

        public static List<Employee> EmployeeList;
    }
}
