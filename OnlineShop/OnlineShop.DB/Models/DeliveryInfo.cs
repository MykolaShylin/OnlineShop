using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models
{
    public class DeliveryInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DeliveryType { get; set; }
        public string City { get; set; }
        public string PostNumber { get; set; }
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string SerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    } 
}
