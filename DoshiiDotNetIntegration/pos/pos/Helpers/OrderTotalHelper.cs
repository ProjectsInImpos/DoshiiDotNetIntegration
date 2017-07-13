using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace pos.Helpers
{
    public static class OrderTotalHelper
    {
        private static decimal CalculateOrderItemTotal(Order order)
        {
            decimal orderTotal = 0M;
            foreach (var item in order.Items)
            {
                orderTotal += item.TotalAfterSurcounts;
            }
            return orderTotal;
        }

        private static void ValueSurcountsOnOrder(Order order)
        {
            foreach (var sur in order.Surcounts)
            {
                if (sur.Type == "percentage")
                {
                    sur.Value = (int) (CalculateOrderItemTotal(order) * (sur.Amount / 100) * 100);
                }
                else
                {
                    sur.Value = (int)sur.Amount * 100;
                }
            }
        }

        public static int CalculateOrderTotal(Order order)
        {
            List<Product> newProductList = new List<Product>();
            foreach (var item in order.Items)
            {
                var newItem = LiveData.ProductList.FirstOrDefault(x => x.PosId == item.PosId);
                if (newItem.Quantity == 0)
                {
                    newItem.Quantity = 1;

                }
                newItem.TotalBeforeSurcounts = newItem.Quantity * newItem.UnitPrice;
                newItem.TotalAfterSurcounts = newItem.TotalBeforeSurcounts;
                newProductList.Add(item);
            }
            order.Items = newProductList;
            decimal orderTotal = 0M;
            ValueSurcountsOnOrder(order);
            orderTotal = CalculateOrderItemTotal(order);
            foreach (var sur in order.Surcounts)
            {
                orderTotal += sur.Value;
            }
            return (int)(orderTotal * 100);
        }
    }
}
