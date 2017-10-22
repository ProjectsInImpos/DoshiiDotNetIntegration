using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace pos.DoshiiImplementation
{
    public class PosDoshiiController
    {
        public DoshiiController Doshii;

        public PosDoshiiController()
        {
            
        }

        public void Initialize(IConfigurationManager manager)
        {
            Doshii = new DoshiiController(manager);
            Doshii.Initialize(LiveData.PosSettings.UseSocketConnection);
        }

        public void GetOrdersFromDoshii()
        {
            var orderList = Doshii.GetOrders();
            if (orderList.Success)
            {
                LiveData.OrdersList = orderList.ReturnObject.ToList();
            }
        }

        public void GetAcceptedOrdersFromDoshii()
        {
            var orderList = Doshii.GetOrdersByStatus("accepted");
            if(orderList.Success)
            {
                LiveData.OrdersList = orderList.ReturnObject.ToList();
            }
        }

        public void UpdateOrder(Order order)
        {
            foreach (var ord in LiveData.OrdersList.Where(x => x.Id == order.Id))
            {
                ord.CheckinId = order.CheckinId;
                ord.Items = order.Items;
                ord.MemberId = order.MemberId;
                ord.Phase = order.Phase;
                ord.Surcounts = order.Surcounts;
                ord.Type = order.Type;
                ord.AvailableEta = order.AvailableEta;
                ord.Consumer = order.Consumer;
                ord.DoshiiId = order.DoshiiId;
                ord.InvoiceId = order.InvoiceId;
                ord.Log = order.Log;
                ord.ManuallyProcessed = order.ManuallyProcessed;
                ord.RejectionCode = order.RejectionCode;
                ord.RequiredAt = order.RequiredAt;
                ord.Version = order.Version;
            }
        }

        public void SyncEmployees()
        {
            try
            {
                var doshiiEmployees = Doshii.GetEmployees();
                if (doshiiEmployees.Success)
                {
                    foreach (var emp in doshiiEmployees.ReturnObject)
                    {
                        if (LiveData.EmployeeList.Exists(e => e.Id == emp.Id))
                        {
                            UpdateEmployee(emp);
                        }
                        else
                        {
                            LiveData.EmployeeList.Add(emp);
                        }
                    }
                }
            }
            catch(Exception Ex)
            {
                
            }
        }

        public void SyncMenu()
        {
            try
            {
                var doshiiMenu = Doshii.GetMenu();
                if (doshiiMenu.Success)
                {
                    SyncProducts(doshiiMenu.ReturnObject.Products.ToList());
                    SyncSurcounts(doshiiMenu.ReturnObject.Surcounts.ToList());
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void UpdateMember(MemberOrg theMember)
        {
            foreach (var mem in LiveData.MemberList.Where(x => x.Id == theMember.Id))
            {
                mem.Address = theMember.Address;
                mem.Email = theMember.Email;
                mem.FirstName = theMember.FirstName;
                mem.LastName = theMember.LastName;
                mem.Name = theMember.Name;
                mem.Phone = theMember.Phone;
                mem.Ref = theMember.Ref;
            }
        }

        public void SyncTables()
        {
            try
            {
                var doshiiTables = Doshii.GetTables();
                if (doshiiTables.Success)
                {
                    foreach (var table in doshiiTables.ReturnObject)
                    {
                        if (LiveData.TablesList.Exists(p => p.Name == table.Name))
                        {
                            UpdateTable(table);
                        }
                        else
                        {
                            LiveData.TablesList.Add(table);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateTable(Table table)
        {
            foreach (var tab in LiveData.TablesList.Where(p => p.Name == table.Name))
            {
                tab.Covers = table.Covers;
                tab.Criteria = table.Criteria;
                tab.IsActive = table.IsActive;
            }
        }

        public void UpdateBooking(Booking booking)
        {
            foreach (var book in LiveData.BookingList.Where(p => p.Id == booking.Id))
            {
                book.CheckinId = booking.CheckinId;
                book.App = booking.App;
                book.Consumer = booking.Consumer;
                book.Covers = booking.Covers;
                book.Date = booking.Date;
                book.TableNames = booking.TableNames;
               
            }
        }

        private void SyncProducts(List<Product> productList)
        {
            foreach (var product in productList)
            {
                if (LiveData.ProductList.Exists(p => p.PosId == product.PosId))
                {
                    UpdateProduct(product);
                }
                else
                {
                    LiveData.ProductList.Add(product);
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            foreach (var pro in LiveData.ProductList.Where(p => p.PosId == product.PosId))
            {
                pro.Description = product.Description;
                pro.IncludedItems = product.IncludedItems;
                pro.MenuDir = product.MenuDir;
                pro.Name = product.Name;
                pro.ProductOptions = product.ProductOptions;
                pro.ProductSurcounts = product.ProductSurcounts;
                pro.Tags = product.Tags;
                pro.Type = product.Type;
                pro.UnitPrice = product.UnitPrice;
                pro.Uuid = product.Uuid;
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            foreach (var emp in LiveData.EmployeeList.Where(e => e.Id == employee.Id))
            {
                emp.Address = employee.Address;
                emp.Email = employee.Email;
                emp.FirstName = employee.FirstName;
                emp.LastName = employee.LastName;
                emp.LocationId = employee.LocationId;
                emp.Phone = employee.Phone;
                emp.PosRef = employee.PosRef;
            }
        }

        private void SyncSurcounts(List<Surcount> surcountList)
        {
            foreach (var surcount in surcountList)
            {
                if (LiveData.SurcountList.Exists(p => p.Id == surcount.Id))
                {
                    UpdateSurcount(surcount);
                }
                else
                {
                    LiveData.SurcountList.Add(surcount);
                }
            }
        }

        public void UpdateSurcount(Surcount surcount)
        {
            foreach (var sur in LiveData.SurcountList.Where(p => p.Id == surcount.Id))
            {
                sur.Amount = surcount.Amount;
                sur.Name = surcount.Name;
                sur.Type = surcount.Type;
                sur.Value = surcount.Value;
            }
        }
    }
}
