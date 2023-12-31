﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShop.DB.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OnlineShop.DB.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ProductCategories Category { get; set; }
        public string Name { get; set; }
        public ProductBrands Brand { get; set; }
        public decimal Cost { get; set; }

        public decimal DiscountCost { get; set; }

        public string Description { get; set; }
        public string? DiscountDescription { get; set; } =  string.Empty;
        public int AmountInStock { get; set; }
        public List<Flavor> Flavors { get; set; } = new List<Flavor>();
        public List<ProductPicture> Pictures { get; set; } = new List<ProductPicture>();
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();        
        public List<FavoriteProduct> FavoriteProducts { get; set; } = new List<FavoriteProduct>();

        [Timestamp]
        public byte[] Concurrency { get; set; }

    }
}
