using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models.Enumerations
{
    public enum ProductBrands
    {
        [Display(Name = "Optimum Nutrition")]
        Optimum_Nutrition,
        [Display(Name = "Scitec Nutrition")]
        Scitec_Nutrition,
        [Display(Name = "Sporter")]
        Sporter,
        [Display(Name = "BSN")]
        BSN,
        [Display(Name = "MST")]
        MST,
        [Display(Name = "Biotech USA")]
        Biotech_USA,
        [Display(Name = "Dymatize")]
        Dymatize,
        [Display(Name = "Rule1")]
        Rule1,
        [Display(Name = "TREC")]
        TREC,
        [Display(Name = "GO ON Nutrition")]
        GO_ON
    }
}
