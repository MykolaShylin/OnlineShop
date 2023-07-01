using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.FeedbackApi.Models
{
    public class AddFeedbackModel
    {
        public int ProductId { get; set; }


        public int UserId { get; set; }


        public string? Text { get; set; }
        
        public int Grade { get; set; }
    }
}
