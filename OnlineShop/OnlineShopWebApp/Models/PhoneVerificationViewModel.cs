namespace OnlineShopWebApp.Models
{
    public class PhoneVerificationViewModel
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }

        public string ReturnUrl { get; set; }
    }
}
