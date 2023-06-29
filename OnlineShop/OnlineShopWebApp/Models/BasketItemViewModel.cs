using OnlineShop.DB.Models;
using System;

namespace OnlineShopWebApp.Models
{
    public class BasketItemViewModel
    {
        public Guid Id { get; set; }
        public ProductViewModel Product { get; set; }

        public ChoosingProductInfoViewModel ProductInfo { get; set; }
        public int Amount { get; set; }

        public decimal Cost
        {
            get
            {
                return Product.Cost * Amount;
            }
        }
    }
}
