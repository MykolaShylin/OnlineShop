using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OnlineShopWebApp.Models
{
    public class BasketViewModel
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }

        public List<BasketItemViewModel> Items { get; set; }

        public bool IsClosed { get; set; }
        public decimal Cost
        {
            get
            {
                return Items?.Sum(x=> x.Cost) ?? 0;
            }
        }
        public decimal Amount
        {
            get
            {
                return Items?.Sum(x => x.Amount) ?? 0;
            }
        }
    }
}
