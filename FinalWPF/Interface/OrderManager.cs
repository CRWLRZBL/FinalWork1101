using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWPF.Interface
{
    public class OrderManager : IOrderManagement
    {        
        private List<OrderDetails> orders = new List<OrderDetails>();

        public OrderDetails GetOrderDetails(int orderId)
        {
            return orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        public void SetDeliveryDate(int orderId, DateTime deliveryDate)
        {
            var order = GetOrderDetails(orderId);
            if (order != null)
            {
                order.DeliveryDate = deliveryDate;
            }
        }

        public void ChangeOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = GetOrderDetails(orderId);
            if (order != null)
            {
                order.Status = newStatus;
            }
        }
    }
}
