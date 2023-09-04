using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models
{
    public class EditBasket
    {
        public Guid ItemId { get; set; }
        public string UserId { get; set; }
        public int FlavorId { get; set; }
    }
}
