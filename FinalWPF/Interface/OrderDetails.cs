using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWPF.Interface
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public OrderStatus Status { get; set; }
        public string ClientFullName { get; set; } // Если заказ оформлен авторизованным клиентом
    }

    public enum OrderStatus
    {
        New,
        Completed
    }
}
