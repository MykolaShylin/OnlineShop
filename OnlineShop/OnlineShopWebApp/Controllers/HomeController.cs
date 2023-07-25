using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Models;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.Services;

namespace OnlineShopWebApp.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(TelegramService telegramService)
        {
        }

        public IActionResult Index(string defaultPassword)
        {
            ViewBag.DefaultPassword = defaultPassword;
            return View();            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contacts()
        {            
            return View();
        }
    }
}
