using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models.Enumerations
{
    public enum ProductCategories
    {
        [Display(Name = "Протеин")]
        Protein,
        [Display(Name = "Казеин")]
        Caseine,
        [Display(Name = "Протеиновые батончики")]
        ProteinBar,
        [Display(Name = "Аминокислоты")]
        Aminoacid,
        [Display(Name = "Креатин")]
        Creatine,
        [Display(Name = "BCAA")]
        BCAA,
        [Display(Name = "Гейнер")]
        Gainer,
        [Display(Name = "Цитрулин")]
        Citruline,
        [Display(Name = "Жиросжигатели")]
        FatBurner,
        [Display(Name = "Здоровые перекусы")]
        HealthySnacks,
        [Display(Name = "Добавки")]
        Supplements,
        [Display(Name = "Витамины для мужчин")]
        ManVitamins,
        [Display(Name = "Витамины для женщин")]
        WomenVitamins,
        [Display(Name = "Мультивитамины")]
        Multivitamins,
        [Display(Name = "Все товары")]
        None
    }
}
