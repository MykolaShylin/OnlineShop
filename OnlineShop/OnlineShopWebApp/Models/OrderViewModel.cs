using Microsoft.VisualBasic;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Models
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public List<BasketItemViewModel> Items { get; set; }
        public DeliveryInfoViewModel deliveryInfo { get; set; }
        public PayInfoViewModel payInfo { get; set; }
        public OrderStatuses orderStatus { get; set; }
        public string OrderDateTime { get; set; }  

        public decimal TotalCost
        {
            get
            {
                return Items?.Sum(x=>x.TotalAmount) ?? 0;
            }
        }
    }
}
