using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models
{
    public class OrderStatusUpdatedEventArgs
    {
        public User User { get; set; }
        public OrderStatuses OldStatus { get; set; }
        public OrderStatuses NewStatus { get; set; }
        public Order Order { get; set; }
    }
}
