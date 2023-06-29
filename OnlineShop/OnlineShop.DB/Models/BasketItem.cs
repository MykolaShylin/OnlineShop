using System;

namespace OnlineShop.DB.Models
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
        public Basket Basket { get; set; }
        public ChoosingProductInfo ProductInfo { get; set; }
    }
}
