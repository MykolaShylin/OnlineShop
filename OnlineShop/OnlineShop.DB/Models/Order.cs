using Microsoft.VisualBasic;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.DB.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public List<BasketItem> Items { get; set; }
        public DeliveryInfo deliveryInfo { get; set; }
        public PayInfo payInfo { get; set; }

        [ConcurrencyCheck]
        public OrderStatuses orderStatus { get; set; }
        public DateTime OrderDateTime { get; set; }

        public Order()
        {
            OrderDateTime = DateTime.UtcNow;

            orderStatus = OrderStatuses.Created;
            Id = Guid.NewGuid();
        }

    }
}
