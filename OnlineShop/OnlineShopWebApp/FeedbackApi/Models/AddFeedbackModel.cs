using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.FeedbackApi.Models
{
    public class AddFeedbackModel
    {
        public int ProductId { get; set; }

        public string Login { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Имя не указано")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Отзыв не написан")]
        public string Text { get; set; }

        public int Grade { get; set; }
    }
}
