using AutoMapper;
using OnlineShop.DB.Models;
using System;

namespace OnlineShopWebApp.Models
{
   
    public class ChoosingProductInfoViewModel
    {        
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int FlavorId { get; set; }

    }
}
