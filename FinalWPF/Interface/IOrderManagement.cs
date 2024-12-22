using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWPF.Interface
{
    public interface IOrderManagement
    {
        OrderDetails GetOrderDetails(int orderId);

        void SetDeliveryDate(int orderId, DateTime deliveryDate);

        void ChangeOrderStatus(int orderId, OrderStatus newStatus);
    }
}
