using System;

namespace OnlineShopWebApp.FeedbackApi.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string? Text { get; set; }

        public int Grade { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
