using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models.Enumerations
{
    public enum OrderStatuses
    {
        [Display(Name = "Создан")]
        Created,
        [Display(Name = "В пути")]
        OnTheWay,
        [Display(Name = "Отменен")]
        Canceled,
        [Display(Name = "Доставлен")]
        Delivered,
        [Display(Name = "Получен покупателем")]
        Recived
    }
}
