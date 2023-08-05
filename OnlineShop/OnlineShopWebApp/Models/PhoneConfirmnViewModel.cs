namespace OnlineShopWebApp.Models
{
    public class PhoneConfirmnViewModel
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }

        public string ReturnUrl { get; set; }
        public string ConfirmCode { get; set;}
    }
}
