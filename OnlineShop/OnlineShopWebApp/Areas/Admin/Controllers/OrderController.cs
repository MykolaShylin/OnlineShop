using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using OnlineShopWebApp.Services;
using OnlineShop.DB.Patterns;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = $"{Constants.AdminRoleName}, {Constants.ModeratorRoleName}")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;

        public OrderController(UserManager<User> userManager, IMapper mapping, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapping = mapping;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> ClosedOrders()
        {
            var closedOrders = await _unitOfWork.ClosedPurchasesDbStorage.GetAllAsync();
            var closedOrdersView = _mapping.Map<List<OrderViewModel>>(closedOrders);
            return View(closedOrdersView);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateOrderStatusAsync(Guid orderId, OrderStatuses orderStatus)
        {
            try
            {
                await _unitOfWork.ClosedPurchasesDbStorage.UpdateStatusAsync(orderId, orderStatus);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(ClosedOrders));
            }
            catch(DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Order)entry.Entity;
                var databaseValues = (Order)entry.GetDatabaseValues().ToObject();

                if (databaseValues.orderStatus != clientValues.orderStatus)
                {
                    ModelState.AddModelError("orderStatus", "Текущее значение: " + EnumHelper.GetDisplayName(databaseValues.orderStatus));
                }
                ModelState.AddModelError(string.Empty, "Статус был изменен другим пользователем в момент вашей работы. Если вы все еще хотите внести изменения в статус заказа, нажмите кнопку Обновить еще раз или вернитесь на исходную страницу");
            }
            return Redirect($"/CustomerOrder?id={orderId}");
        }
        public async Task<IActionResult> CustomerOrder(Guid id)
        {
            var order = await _unitOfWork.ClosedPurchasesDbStorage.TryGetByIdAsync(id);
            var customer = await _userManager.FindByIdAsync(order.deliveryInfo.CustomerId);
            var orderView = _mapping.Map<OrderViewModel>(order);
            var customerView = _mapping.Map<UserViewModel>(customer);
            ViewBag.CustomerInfo = customerView;
            return View(orderView);
        }        
    }
}
