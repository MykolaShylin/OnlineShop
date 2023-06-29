using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class DiscountViewModel
    {
        public int Id { get; set; }

        public List<ProductViewModel> Products { get; set; }
        public int DiscountPercent { get; set; }
        
    }
}
