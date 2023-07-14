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
                return DiscountCost * Amount;
            }
        }
        public decimal DiscountCost
        {
            get
            {
                return decimal.Ceiling(((Product.Cost * 100) * (100 - ProductInfo.DiscountPercent) / 100) / 100);
            }
        }
    }
}
