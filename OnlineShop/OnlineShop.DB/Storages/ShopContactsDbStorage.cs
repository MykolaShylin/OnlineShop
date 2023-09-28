
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web;
using Nancy.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace OnlineShop.DB.Storages
{
    public class ShopContactsDbStorage : IGoogleMap
    {
        private readonly DataBaseContext _dataBaseContext;
        public ShopContactsDbStorage(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }
        public List<GoogleMapShopInfo> GetAll()
        {
            return _dataBaseContext.ShopContacts.ToList();
        }
    }
}
