using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models
{
    public class Login
    {
        public string NameLogin { get; set; }

        public string Password { get; set; }
        public bool IsRemember { get; set; }

        public string ReturnUrl { get; set; }
    }
}
