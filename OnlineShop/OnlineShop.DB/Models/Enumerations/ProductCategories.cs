using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DB.Models.Enumerations
{
    public enum ProductCategories
    {
        [Display(Name = "Протеин")]
        Sport_Protein,
        [Display(Name = "Казеин")]
        Sport_Caseine,
        [Display(Name = "Протеиновые батончики")]
        Sport_ProteinBar,
        [Display(Name = "Аминокислоты")]
        Sport_Aminoacid,
        [Display(Name = "Креатин")]
        Sport_Creatine,
        [Display(Name = "BCAA")]
        Sport_BCAA,
        [Display(Name = "Гейнер")]
        Sport_Gainer,
        [Display(Name = "Цитрулин")]
        Sport_Citruline,
        [Display(Name = "Жиросжигатели")]
        Sport_FatBurner,
        [Display(Name = "Гранола")]
        HealthyFood_Granola,
        [Display(Name = "Арахисовая паста")]
        HealthyFood_Peanut_Butter,
        [Display(Name = "Протеиновые чипсы")]
        HealthyFood_Proteine_Chips,
        [Display(Name = "Мюсли")]
        HealthyFood_Muesli,
        [Display(Name = "Протеиновые панкейки")]
        HealthyFood_Protein_Puncakes,
        [Display(Name = "Протеиновая выпечка")]
        HealthyFood_Protein_Muffin,
        [Display(Name = "Витамины для мужчин")]
        Vitamins_ManVitamins,
        [Display(Name = "Витамины для женщин")]
        Vitamins_WomenVitamins,
        [Display(Name = "Мультивитамины")]
        Vitamins_Multivitamins,
        [Display(Name = "Добавки")]
        Supplements,
        [Display(Name = "Все товары")]
        None
    }
}
