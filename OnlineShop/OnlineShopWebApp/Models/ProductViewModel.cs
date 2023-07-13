using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShopWebApp.FeedbackApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShopWebApp.Models
{
    public class ProductViewModel
    {
        
        public int Id { get; set; }
        public ProductCategories Category { get; set; }

        [Required(ErrorMessage = "Название не указано")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Бренд не указан")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Допускается от 3 до 30 символов")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Цена не указана")]        
        public decimal Cost { get; set; }
        public List<FlavorViewModel> Flavors { get; set; }

        [Required(ErrorMessage = "Описание не добавлено")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Допускается от 20 до 2000 символов")]
        public string Description { get; set; }
        public string DiscountDescription { get; set; }
        public decimal DiscountCost { get; set; }
        public List<ProductPicture> Pictures { get; set; }

        public ProductPicture Picture => Pictures?.Count > 0 ? Pictures[0] : null;

        [Required(ErrorMessage = "Количество не указано")]
        public int AmountInStock { get; set; }

        public List<IFormFile> UploadedFile { get; set; }

        [Timestamp]
        public byte[] Concurrency { get; set; }

        public List<FeedbackViewModel> Feedbacks { get; set; }
    }
}
