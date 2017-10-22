using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using pos.Models;

namespace pos.DoshiiImplementation
{
    public class OrderingManager : IOrderingManager
    {
        private PosDoshiiController DoshiiController;

        public OrderingManager(PosDoshiiController doshiiController)
        {
            if (doshiiController == null)
            {
                throw new NullReferenceException("doshii controller");
            }
            DoshiiController = doshiiController;
        }

        public Order RetrieveOrder(string posOrderId)
        {
            Order order = LiveData.OrdersList.FirstOrDefault(o => o.Id == posOrderId);
            if (order != null)
            {
                return order;
            }
            else
            {
                throw new OrderDoesNotExistOnPosException();
            }
        }

        public void RecordOrderVersion(string posOrderId, string version)
        {
            if (LiveData.OrderDataList.Any(o => o.OrderId == posOrderId))
            {
                foreach (var ord in LiveData.OrderDataList.Where(o => o.OrderId == posOrderId))
                {
                    ord.Version = version;
                }
            }
            else
            {
                LiveData.OrderDataList.Add(new OrderDataInfo(posOrderId, version));
            }
        }

        public string RetrieveOrderVersion(string posOrderId)
        {
            var ord = LiveData.OrderDataList.FirstOrDefault(o => o.OrderId == posOrderId);
            if (ord == null)
            {
                return string.Empty;
            }
            else
            {
                return ord.Version;
            }
        }

        public void RecordCheckinForOrder(string posOrderId, string checkinId)
        {
            if (LiveData.OrderDataList.Any(o => o.OrderId == posOrderId))
            {
                foreach (var ord in LiveData.OrderDataList.Where(o => o.OrderId == posOrderId))
                {
                    ord.Checkin = checkinId;
                }
            }
            else
            {
                var orderData = new OrderDataInfo();
                orderData.OrderId = posOrderId;
                orderData.Checkin = checkinId;
                LiveData.OrderDataList.Add(orderData);
            }
        }

        public string RetrieveCheckinIdForOrder(string posOrderId)
        {
            var ord = LiveData.OrderDataList.FirstOrDefault(o => o.OrderId == posOrderId);
            if (ord != null)
            {
                return string.Empty;
            }
            else
            {
                return ord.Checkin;
            }
        }

        public void ConfirmNewDeliveryOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList)
        {
            ConfirmDoshiiOrder(order, transactionList);
        }

        public void ConfirmNewDeliveryOrder(Order order, Consumer consumer)
        {
            ConfirmDoshiiOrder(order, new List<Transaction>());
        }

        public void ConfirmNewPickupOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList)
        {
            ConfirmDoshiiOrder(order, transactionList);
        }

        public void ConfirmNewDineInOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList)
        {
            ConfirmDoshiiOrder(order, transactionList);
        }

        public void ConfirmNewPickupOrder(Order order, Consumer consumer)
        {
            ConfirmDoshiiOrder(order, new List<Transaction>());
        }

        public void ConfirmNewDineInOrder(Order order, Consumer consumer)
        {
            ConfirmDoshiiOrder(order, new List<Transaction>());
        }

        public void ConfirmNewUnknownTypeOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList)
        {
            ConfirmDoshiiOrder(order, transactionList);
        }

        public void ConfirmNewUnknownTypeOrder(Order order, Consumer consumer)
        {
            ConfirmDoshiiOrder(order, new List<Transaction>());
        }

        public void ProcessVenueCanceledOrderUpdate(Order order)
        {
            LiveData.OrdersList.RemoveAll(x => x.Id == order.Id);
        }

        private void ConfirmDoshiiOrder(Order order, IEnumerable<Transaction> transactionList)
        {
            if (transactionList.Any())
            {
                if (LiveData.PosSettings.ConfirmAllTransactions)
                {
                    if (LiveData.PosSettings.ConfirmAllOrders)
                    {
                        order.Status = "accepted";
                        addPosIdToOpenItems(order);
                        order.Id = LiveData.PosSettings.OrderNumber.ToString();
                        LiveData.OrdersList.Add(order);
                        DoshiiController.Doshii.AcceptOrderAheadCreation(order);

                    }
                    else
                    {
                        order.Status = "rejected";
                        LiveData.OrdersList.RemoveAll(x => x.Id == order.Id);
                        DoshiiController.Doshii.RejectOrderAheadCreation(order);
                    }
                }
            }
            else
            {
                if (LiveData.PosSettings.ConfirmAllOrders)
                {
                    order.Status = "accepted";
                    order.Id = LiveData.PosSettings.OrderNumber.ToString();
                    addPosIdToOpenItems(order);
                    LiveData.OrdersList.Add(order);
                    DoshiiController.Doshii.AcceptOrderAheadCreation(order);
                }
                else
                {
                    order.Status = "rejected";
                    LiveData.OrdersList.RemoveAll(x => x.Id == order.Id);
                    DoshiiController.Doshii.RejectOrderAheadCreation(order);
                }
            }
            
        }

        private void addPosIdToOpenItems(Order order)
        {
            foreach (var item in order.Items)
            {
                if (item.PosId == null || string.IsNullOrEmpty(item.PosId))
                {
                    //get openItem
                    var openItem = LiveData.ProductList.FirstOrDefault(x => x.PosId == "DoshiiOI");
                    if (openItem == null)
                    {
                        var newItem = new Product()
                        {
                            Description = "DoshiiOpenItem",
                            Name = "DoshiiOpenItem",
                            PosId = "DoshiiOI"
                        };
                        LiveData.ProductList.Add(newItem);
                        openItem = newItem;
                    }
                    item.PosId = "DoshiiOI";
                }
            }
        }
    }
}
