using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OnlineShop.DB.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class PayInfoViewModel
    {
        public int Id { get; set; }
        public string PayType { get; set; }
    }
}
