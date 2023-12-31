﻿using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class MainPageProductsViewModel
    {
        public int Id { get; set; }
        public ProductCategories Category { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Cost { get; set; }
        public decimal DiscountCost { get; set; }
        public int AmountInStock { get; set; }

        public Flavor Flavor { get; set; }
        public List<ProductPicture> Pictures { get; set; }

        public ProductPicture Picture => Pictures?.Count > 0 ? Pictures[0] : null;

        public double Rating { get; set; }

        public bool IsInFavorites { get; set; } = false;

        public int PageNumber { get; set; }

        public int RatingFillPercent
        {
            get
            {
                return Convert.ToInt32((Rating - Math.Truncate(Rating)) * 100);
            }
        }
    }
}
