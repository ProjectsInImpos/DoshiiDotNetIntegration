using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using Newtonsoft.Json;
using pos.Models;

namespace pos.Helpers
{
    public static class DataPersistanceHelper
    {
        public static void RetreiveDataFromStore()
        {
            LiveData.AppsList = JsonConvert.DeserializeObject<List<App>>(ReadDataFromFile("apps.json"));
            if (LiveData.AppsList == null)
            {
                LiveData.AppsList = new List<App>();
            }
            
            LiveData.BookingList = JsonConvert.DeserializeObject<List<Booking>>(ReadDataFromFile("bookings.json"));
            if (LiveData.BookingList == null)
            {
                LiveData.BookingList = new List<Booking>();
            }
            
            LiveData.ConsumerList = JsonConvert.DeserializeObject<List<Consumer>>(ReadDataFromFile("consumers.json"));
            if (LiveData.ConsumerList == null)
            {
                LiveData.ConsumerList = new List<Consumer>();
            }
            
            LiveData.ProductList = JsonConvert.DeserializeObject<List<Product>>(ReadDataFromFile("Items.json"));
            if (LiveData.ProductList == null)
            {
                LiveData.ProductList = new List<Product>();
            }
            
            LiveData.MemberList = JsonConvert.DeserializeObject<List<MemberOrg>>(ReadDataFromFile("members.json"));
            if (LiveData.MemberList == null)
            {
                LiveData.MemberList = new List<MemberOrg>();
            }
            
            LiveData.OrdersList = JsonConvert.DeserializeObject<List<Order>>(ReadDataFromFile("orders.json"));
            if (LiveData.OrdersList == null)
            {
                LiveData.OrdersList = new List<Order>();
            }
            
            LiveData.PosSettings = JsonConvert.DeserializeObject<PosSettings>(ReadDataFromFile("settings.json"));
            if (LiveData.PosSettings == null)
            {
                LiveData.PosSettings = new PosSettings();
            }
            LiveData.TablesList = JsonConvert.DeserializeObject<List<Table>>(ReadDataFromFile("tables.json"));
            if (LiveData.TablesList == null)
            {
                LiveData.TablesList = new List<Table>();
            }
            LiveData.OrderDataList = JsonConvert.DeserializeObject<List<OrderDataInfo>>(ReadDataFromFile("orderData.json"));
            if (LiveData.OrderDataList == null)
            {
                LiveData.OrderDataList = new List<OrderDataInfo>();
            }
            LiveData.TransactionList = JsonConvert.DeserializeObject<List<Transaction>>(ReadDataFromFile("transactions.json"));
            if (LiveData.TransactionList == null)
            {
                LiveData.TransactionList = new List<Transaction>();
            }
            LiveData.TransactionInfoList = JsonConvert.DeserializeObject<List<TransactionInfo>>(ReadDataFromFile("transactionVersions.json"));
            if (LiveData.TransactionInfoList == null)
            {
                LiveData.TransactionInfoList = new List<TransactionInfo>();
            }
            LiveData.ChecinList = JsonConvert.DeserializeObject<List<Checkin>>(ReadDataFromFile("checkinData.json"));
            if (LiveData.ChecinList == null)
            {
                LiveData.ChecinList = new List<Checkin>();
            }
            LiveData.SurcountList = JsonConvert.DeserializeObject<List<Surcount>>(ReadDataFromFile("surcountData.json"));
            if (LiveData.SurcountList == null)
            {
                LiveData.SurcountList = new List<Surcount>();
            }
            LiveData.LocationList = JsonConvert.DeserializeObject<List<Location>>(ReadDataFromFile("locationData.json"));
            if (LiveData.LocationList == null)
            {
                LiveData.LocationList = new List<Location>();
            }
            LiveData.EmployeeList = JsonConvert.DeserializeObject<List<Employee>>(ReadDataFromFile("employeeData.json"));
            if (LiveData.EmployeeList == null)
            {
                LiveData.EmployeeList = new List<Employee>();
            }
        }

        public static void SaveDataToStore()
        {
            SaveDataToFile("apps.json", JsonConvert.SerializeObject(LiveData.AppsList));
            SaveDataToFile("bookings.json", JsonConvert.SerializeObject(LiveData.BookingList));
            SaveDataToFile("consumers.json", JsonConvert.SerializeObject(LiveData.ConsumerList));
            SaveDataToFile("Items.json", JsonConvert.SerializeObject(LiveData.ProductList));
            SaveDataToFile("members.json", JsonConvert.SerializeObject(LiveData.MemberList));
            SaveDataToFile("orders.json", JsonConvert.SerializeObject(LiveData.OrdersList));
            SaveDataToFile("settings.json", JsonConvert.SerializeObject(LiveData.PosSettings));
            SaveDataToFile("tables.json", JsonConvert.SerializeObject(LiveData.TablesList));
            SaveDataToFile("orderData.json", JsonConvert.SerializeObject(LiveData.OrderDataList));
            SaveDataToFile("transactions.json", JsonConvert.SerializeObject(LiveData.TransactionList));
            SaveDataToFile("transactionVersions.json", JsonConvert.SerializeObject(LiveData.TransactionInfoList));
            SaveDataToFile("checkinData.json", JsonConvert.SerializeObject(LiveData.ChecinList));
            SaveDataToFile("surcountData.json", JsonConvert.SerializeObject(LiveData.SurcountList));
            SaveDataToFile("locationData.json", JsonConvert.SerializeObject(LiveData.LocationList));
            SaveDataToFile("employeeData.json", JsonConvert.SerializeObject(LiveData.EmployeeList));
        }

        private static string ReadDataFromFile(string fileName)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"PosStorage\" + fileName);
            string file = "";
            if (File.Exists(path))
            {
                file = File.ReadAllText(path);
            }
            
            return file;
        }

        private static bool SaveDataToFile(string fileName, string dataToWrite)
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),@"PosStorage\" + fileName);
                File.WriteAllText(path, dataToWrite);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private static object syncLock = new object();
        
        public static bool WriteLogTofile(string message)
        {
            lock (syncLock)
            {
                string messageToLog = DateTime.Now.ToString() + " : " + message;
                try
                {
                    string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        @"PosStorage\" + "lpcmpLog.txt");
                    File.AppendAllText(path, messageToLog);
                    File.AppendAllText(path, Environment.NewLine);
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
