using Microsoft.AspNetCore.Identity;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.FeedbackApi;
using OnlineShopWebApp.FeedbackApi.Models;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace OnlineShopWebApp.Helpers
{
    public class Mapping
    {
        public static List<DiscountViewModel> ConvertToDiscountsProductsView(List<Discount> discounts)
        {
            var discountsView = new List<DiscountViewModel>();
            foreach (var discount in discounts)
            {
                var discountView = new DiscountViewModel
                {
                    Id = discount.Id,
                    Products = ConvertToProductsView(discount.Products),
                    DiscountPercent = discount.DiscountPercent
                };
                discountsView.Add(discountView);
            }
            return discountsView;
        }
        public static List<Discount> ConvertToDiscountsProductsDb(List<DiscountViewModel> discounts)
        {
            var discountsView = new List<Discount>();
            foreach (var discount in discounts)
            {
                var discountView = new Discount
                {
                    Id = discount.Id,
                    Products = ConvertToProductsDb(discount.Products),
                    DiscountPercent = discount.DiscountPercent
                };
                discountsView.Add(discountView);
            }
            return discountsView;
        }

        public static Discount ConvertToDiscountDb(DiscountViewModel discount)
        {
            return new Discount
            {
                Id = discount.Id,
                DiscountPercent = discount.DiscountPercent,
                Products = Mapping.ConvertToProductsDb(discount.Products)
            };
        }
        public static DiscountViewModel ConvertToDiscountView(Discount discount)
        {
            return new DiscountViewModel
            {
                Id = discount.Id,
                DiscountPercent = discount.DiscountPercent,
                Products = Mapping.ConvertToProductsView(discount.Products)
            };
        }
        public static List<DiscountViewModel> ConvertToDiscountsView(List<Discount> discounts)
        {
            var discountsView = new List<DiscountViewModel>();
            foreach (var discount in discounts)
            {
                var discountView = new DiscountViewModel
                {
                    Id = discount.Id,
                    DiscountPercent = discount.DiscountPercent,
                    Products = Mapping.ConvertToProductsView(discount.Products)
                };
                discountsView.Add(discountView);
            }
            return discountsView;
        }
        public static OrderViewModel ConvertToOrderView(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                Items = ConvertToBasketItemsView(order.Items),
                payInfo = ConvertToPayInfoView(order.payInfo),
                deliveryInfo = ConvertToDeliveryInfoView(order.deliveryInfo),
                orderStatus = order.orderStatus,
                OrderDateTime = order.OrderDateTime
            };
        }
        public static List<OrderViewModel> ConvertToOrdersView(List<Order> orders)
        {
            var ordersView = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                var orderViewModel = new OrderViewModel
                {
                    Id = order.Id,
                    Items = ConvertToBasketItemsView(order.Items),
                    payInfo = ConvertToPayInfoView(order.payInfo),
                    deliveryInfo = ConvertToDeliveryInfoView(order.deliveryInfo),
                    orderStatus = order.orderStatus,
                    OrderDateTime = order.OrderDateTime
                };
                ordersView.Add(orderViewModel);
            }
            return ordersView;
        }

        private static DeliveryInfoViewModel ConvertToDeliveryInfoView(DeliveryInfo deliveryInfo)
        {
            return new DeliveryInfoViewModel
            {
                Id = deliveryInfo.Id,
                DeliveryType = deliveryInfo.DeliveryType,
                City = deliveryInfo.City,
                PostNumber = deliveryInfo.PostNumber,
                CustomerId = deliveryInfo.CustomerId,
                Name = deliveryInfo.Name,
                SerName = deliveryInfo.SerName,
                Phone = deliveryInfo.Phone,
                Email = deliveryInfo.Email
            };
        }

        private static PayInfoViewModel ConvertToPayInfoView(PayInfo payInfo)
        {
            return new PayInfoViewModel
            {
                Id = payInfo.Id,
                PayType = payInfo.PayType
            };
        }

        public static Order ConvertToOrderDb(OrderViewModel order, List<BasketItem> basketItems)
        {
            return new Order
            {
                Items = basketItems,
                payInfo = ConvertToPayInfoDb(order.payInfo),
                deliveryInfo = ConvertToDeliveryInfoDb(order.deliveryInfo),
                orderStatus = order.orderStatus
            };
        }
        public static DeliveryInfo ConvertToDeliveryInfoDb(DeliveryInfoViewModel payInfo)
        {
            return new DeliveryInfo
            {
                Id = payInfo.Id,
                DeliveryType = payInfo.DeliveryType,
                City = payInfo.City,
                PostNumber = payInfo.PostNumber,
                CustomerId = payInfo.CustomerId,
                Name = payInfo.Name,
                SerName = payInfo.SerName,
                Phone = payInfo.Phone,
                Email = payInfo.Email
            };
        }
        public static PayInfo ConvertToPayInfoDb(PayInfoViewModel payInfo)
        {
            return new PayInfo
            {
                Id = payInfo.Id,
                PayType = payInfo.PayType
            };
        }
        public static Role ConvertToRoleDb(RoleViewModel role)
        {
            return new Role
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            };
        }
        public static RoleViewModel ConvertToRoleView(Role role)
        {
            return new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }
        public static List<RoleViewModel> ConvertToRolesView(List<Role> roles)
        {
            var rolesView = new List<RoleViewModel>();
            foreach (var role in roles)
            {
                var viewModel = ConvertToRoleView(role);
                rolesView.Add(viewModel);
            }
            return rolesView;
        }
        public static List<UserViewModel> ConvertToUsersView(List<User> users)
        {
            var usersView = new List<UserViewModel>();
            foreach (var user in users)
            {
                var viewModel = ConvertToUserView(user);
                usersView.Add(viewModel);
            }
            return usersView;
        }
        public static UserViewModel ConvertToUserView(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Login = user.UserName,
                Name = user.RealName,
                SerName = user.SerName,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Avatar = user.Avatar,
                NikName = user.NikName
            };
        }

        public static EditUserViewModel ConvertToUserEditInfoView(User user)
        {
            return new EditUserViewModel
            {
                Id = user.Id,
                Login = user.UserName,
                Name = user.RealName,
                SerName = user.SerName,
                Phone = user.PhoneNumber,
                Email = user.Email,
                ConcurrencyStamp = user.ConcurrencyStamp,
                NikName = user.NikName
            };
        }
        public static User ConvertToUserDb(UserViewModel user)
        {
            return new User
            {
                RealName = user.Name,
                UserName = user.Login,
                SerName = user.SerName,
                PhoneNumber = user.Phone,
                Email = user.Email,
                Avatar = user.Avatar,
                NikName = user.NikName
            };
        }

        public static EditProductDiscountViewModel ConvertToEditDiscountView(Product product, Discount discount, List<Discount> discounts)
        {
            return new EditProductDiscountViewModel
            {
                Product = ConvertToProductView(product),
                Discount = ConvertToDiscountView(discount),
                Discounts = ConvertToDiscountsProductsView(discounts)
            };
        }
        public static ProductViewModel ConvertToProductView(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Category = product.Category,
                Name = product.Name,
                Brand = product.Brand,
                Cost = product.Cost,
                Flavors = ConvertToFlavorsView(product.Flavors),
                Description = product.Description,
                AmountInStock = product.AmountInStock,
                Pictures = product.Pictures,
                DiscountDescription = product.DiscountDescription ?? string.Empty,
                DiscountCost = product.DiscountCost,
                Concurrency = product.Concurrency,
            };
        }

        public static List<FeedbackViewModel> ConvertToFeedbacksView(List<Feedback> feedbacks)
        {
            var feedbacksViewModel = new List<FeedbackViewModel>();
            foreach (var feedback in feedbacks)
            {
                feedbacksViewModel.Add(ConvertFeedbackViewModel(feedback));
            }
            return feedbacksViewModel;
        }
        public static FeedbackViewModel ConvertFeedbackViewModel(Feedback feedback)
        {
            return new FeedbackViewModel
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                UserName= feedback.UserName,
                Text = feedback.Text,
                Grade = feedback.Grade,
                CreateDate = feedback.CreateDate,
            };
        }

        public static List<ComparingProductsViewModel> ConvertToComparerView(List<ComparingProducts> products)
        {
            var productsView = new List<ComparingProductsViewModel>();
            foreach (var product in products)
            {
                var viewModel = new ComparingProductsViewModel
                {
                    Id = product.Id,
                    UserId = product.UserId,
                    Product = ConvertToProductView(product.Product),
                    Flavor = ConvertToFlavorView(product.Flavor)
                };
                productsView.Add(viewModel);
            }
            return productsView;
        }

        public static List<Product> ConvertToProductsDb(List<ProductViewModel> products)
        {
            var productsView = new List<Product>();
            foreach (var product in products)
            {
                Product viewModel = ConvertToProductDb(product);
                productsView.Add(viewModel);
            }
            return productsView;
        }
        public static List<ProductViewModel> ConvertToProductsView(List<Product> products)
        {
            var productsView = new List<ProductViewModel>();
            foreach (var product in products)
            {
                ProductViewModel viewModel = ConvertToProductView(product);
                productsView.Add(viewModel);
            }
            return productsView;
        }
        public static List<FlavorViewModel> ConvertToFlavorsView(List<Flavor> flavors)
        {
            var flavorsViewModel = new List<FlavorViewModel>();
            foreach (var flavor in flavors)
            {
                var flavorViewModel = new FlavorViewModel
                {
                    Id = flavor.Id,
                    Name = flavor.Name
                };
                flavorsViewModel.Add(flavorViewModel);
            }
            return flavorsViewModel;
        }

        public static ChoosingProductInfo ConvertToProductInfoDb(ChoosingProductInfoViewModel product)
        {
            return new ChoosingProductInfo
            {
                Id = product.Id,
                ProductId = product.Id,
                FlavorId = product.FlavorId
            };
        }
        public static ChoosingProductInfoViewModel ConvertToProductInfoView(ChoosingProductInfo product)
        {
            return new ChoosingProductInfoViewModel
            {
                Id = product.Id,
                ProductId = product.ProductId,
                FlavorId = product.FlavorId
            };
        }
        public static BasketViewModel ConvertToBasketView(Basket basket)
        {
            return basket == null ? null : new BasketViewModel
            {
                Id = basket.Id,
                CustomerId = basket.CustomerId,
                Items = ConvertToBasketItemsView(basket.Items),
                IsClosed = basket.IsClosed,
            };
        }

        public static List<BasketItem> ConvertToBasketItemsDb(List<BasketItemViewModel> items)
        {
            if (items == null)
            {
                return null;
            }
            var basketItems = new List<BasketItem>();
            foreach (var item in items)
            {
                var newItem = new BasketItem
                {
                    Id = item.Id,
                    Product = ConvertToProductDb(item.Product),
                    ProductInfo = ConvertToProductInfoDb(item.ProductInfo),
                    Amount = item.Amount
                };
                basketItems.Add(newItem);
            }
            return basketItems;
        }
        public static List<BasketItemViewModel> ConvertToBasketItemsView(List<BasketItem> items)
        {
            var basketItems = new List<BasketItemViewModel>();
            foreach (var item in items)
            {
                var newItem = new BasketItemViewModel
                {
                    Id = item.Id,
                    Product = ConvertToProductView(item.Product),
                    ProductInfo = ConvertToProductInfoView(item.ProductInfo),
                    Amount = item.Amount
                };
                basketItems.Add(newItem);
            }
            return basketItems;
        }
        public static Product ConvertToProductDb(ProductViewModel product)
        {
            return new Product
            {
                Id = product.Id,
                Category = product.Category,
                Name = product.Name,
                Brand = product.Brand,
                Cost = product.Cost,
                Flavors = ConvertToFlavorsDb(product.Flavors),
                Description = product.Description,
                AmountInStock = product.AmountInStock,
                Pictures = product.Pictures,
                DiscountDescription = product.DiscountDescription ?? string.Empty,
                DiscountCost = product.DiscountCost,
                Concurrency = product.Concurrency
            };
        }

        public static FlavorViewModel ConvertToFlavorView(Flavor flavor)
        {
            return new FlavorViewModel
            {
                Id = flavor.Id,
                Name = flavor.Name
            };
        }
        public static List<Flavor> ConvertToFlavorsDb(List<FlavorViewModel> flavors)
        {
            var flavorsViewModel = new List<Flavor>();
            foreach (var flavor in flavors)
            {
                var flavorViewModel = new Flavor
                {
                    Id = flavor.Id,
                    Name = flavor.Name
                };
                flavorsViewModel.Add(flavorViewModel);
            }
            return flavorsViewModel;
        }
    }
}
