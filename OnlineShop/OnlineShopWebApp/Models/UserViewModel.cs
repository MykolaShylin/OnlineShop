using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Логин не указан")]
        [StringLength(30, MinimumLength =3, ErrorMessage ="Допускается от 3 до 30 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Имя не указано")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Фамилия не указана")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string SerName { get; set; }

        [Required(ErrorMessage = "Никнейм не указан")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string NikName { get; set; }

        [Required(ErrorMessage = "Номер телефона не указан")]
        [Phone(ErrorMessage = "Номер должен начинаться с \"+\" и иметь код страны")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email не указан")]
        [EmailAddress(ErrorMessage = "Формат Email не верный")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль не указан")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 20 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Повторный пароль не указан")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        public IFormFile UploadedAvatar { get; set; }
        public string Avatar { get; set; }

    }
}
