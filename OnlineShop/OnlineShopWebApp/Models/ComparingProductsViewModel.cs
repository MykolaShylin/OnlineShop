using OnlineShop.DB.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class ComparingProductsViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ProductViewModel Product { get; set; }
        public FlavorViewModel Flavor { get; set; }
    }
}
