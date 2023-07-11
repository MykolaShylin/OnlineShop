using AutoMapper;
using OnlineShop.DB.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    [AutoMap(typeof(Role))]
    public class RoleViewModel
    {

        public string Id { get; set; }

        [Required(ErrorMessage = "Название не указано")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание не добавлено")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 200 символов")]
        public string Description { get; set; }
    }
}
