using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models
{
    public class ChoosingProductInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DiscountPercent  { get; set; }

        public decimal Cost { get; set; }
        public int FlavorId { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    }
}
