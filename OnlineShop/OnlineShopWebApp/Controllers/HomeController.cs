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
using AutoMapper;
using ReturnTrue.AspNetCore.Identity.Anonymous;

namespace OnlineShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGoogleMap _googleMapContacts;
        private readonly IMapper _mapping;
        public HomeController(IGoogleMap googleMapContacts = null, IMapper mapping = null)
        {
            _googleMapContacts = googleMapContacts;
            _mapping = mapping;
        }

        public IActionResult Index()
        {
            return View();            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            var contacts = _googleMapContacts.GetAll();
            var viewModel = _mapping.Map<List<GoogleMapShopInfoViewModel>>(contacts);
            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetContacts()
        {
            var contacts = _googleMapContacts.GetAll();

            return Json(contacts);
        }
    }
}
