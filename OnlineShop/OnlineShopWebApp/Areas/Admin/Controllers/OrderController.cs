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

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class OrderController : Controller
    {
        private IPurchases _closedPurchases;
        private readonly UserManager<User> _userManager;

        public OrderController(IPurchases closedPurchases, UserManager<User> userManager)
        {
            _closedPurchases = closedPurchases;
            _userManager = userManager;
        }
        public async Task<IActionResult> ClosedOrders()
        {
            var closedOrders = await _closedPurchases.GetAllAsync();
            var closedOrdersView = Mapping.ConvertToOrdersView(closedOrders);
            return View(closedOrdersView);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateOrderStatusAsync(Guid orderId, OrderStatuses orderStatus)
        {
            try
            {
                await _closedPurchases.UpdateStatusAsync(orderId, orderStatus);
                return RedirectToAction("ClosedOrders");
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
            var order = await _closedPurchases.TryGetByIdAsync(id);
            var customer = await _userManager.FindByIdAsync(order.deliveryInfo.CustomerId);
            var orderView = Mapping.ConvertToOrderView(order);
            var customerView = Mapping.ConvertToUserView(customer);
            ViewBag.CustomerInfo = customerView;
            return View(orderView);
        }        
    }
}
