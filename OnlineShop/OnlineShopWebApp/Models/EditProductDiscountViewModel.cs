using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class EditProductDiscountViewModel
    {
        public ProductViewModel Product { get; set; }
        public DiscountViewModel Discount { get; set; }
        public List<DiscountViewModel> Discounts { get; set; }

    }
}
