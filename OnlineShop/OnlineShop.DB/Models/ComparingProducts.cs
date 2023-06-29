using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models
{
    public class ComparingProducts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public string UserId { get; set; }
        public Product Product { get; set; }
        
        public Flavor Flavor { get; set; }

    }
}
