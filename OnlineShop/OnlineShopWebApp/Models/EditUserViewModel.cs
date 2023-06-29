using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Логин не указан")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Имя не указано")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Никнейм не указан")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string NikName { get; set; }

        [Required(ErrorMessage = "Фамилия не указана")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string SerName { get; set; }
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email не указан")]
        [EmailAddress(ErrorMessage = "Формат Email не верный")]
        public string Email { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}
