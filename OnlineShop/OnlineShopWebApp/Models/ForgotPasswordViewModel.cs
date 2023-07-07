using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
