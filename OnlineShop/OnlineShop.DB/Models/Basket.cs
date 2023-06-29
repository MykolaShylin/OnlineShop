using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OnlineShop.DB.Models
{
    public class Basket
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }

        public List<BasketItem> Items { get; set; }

        public bool IsClosed { get; set; }
        public Basket() 
        {
            Items = new List<BasketItem>();
            IsClosed = false;
        }
    }
}
