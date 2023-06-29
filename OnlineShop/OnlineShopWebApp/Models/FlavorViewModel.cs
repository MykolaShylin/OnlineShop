using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Models
{
    public class FlavorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкус не указан")]
        public string Name { get; set; }
    }
}
