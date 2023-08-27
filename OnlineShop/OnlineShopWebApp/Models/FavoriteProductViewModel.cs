using OnlineShop.DB.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class FavoriteProductViewModel
    {
        public int Id { get; set; }

        public List<ProductViewModel> Products { get; set; }
        public string UserId { get; set; }
    }
}
