using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
    public static class Mapping
    {

        public static List<T2> MappToViewModelParameters<T1,T2>(this List<T1> parameters)
        {
            var mapperConfig = new MapperConfiguration(configure => configure.CreateMap<T1, T2>());

            var mapper = new Mapper(mapperConfig);

            return mapper.Map<List<T2>>(parameters);
        }

        public static T2 MappToViewModelParameter <T1, T2>(this T1 parameter)
        {
            var mapperConfig = new MapperConfiguration(configure => configure.CreateMap<T1, T2>());

            var mapper = new Mapper(mapperConfig);

            return mapper.Map<T2>(parameter);
        }

        public static T2 MappToDbParameter<T1, T2>(this T1 parameter)
        {
            var mapperConfig = new MapperConfiguration(configure => configure.CreateMap<T1, T2>());

            var mapper = new Mapper(mapperConfig);

            return mapper.Map<T2>(parameter);
        }

        public static List<T2> MappToDbParameters<T1, T2>(this List<T1> parameters)
        {
            var mapperConfig = new MapperConfiguration(configure => configure.CreateMap<T1, T2>());

            var mapper = new Mapper(mapperConfig);

            return mapper.Map<List<T2>>(parameters);
        }

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
                payInfo = order.payInfo.MappToViewModelParameter<PayInfo, PayInfoViewModel>(),
                deliveryInfo = order.deliveryInfo.MappToViewModelParameter<DeliveryInfo, DeliveryInfoViewModel>(),
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
                    payInfo = order.payInfo.MappToViewModelParameter<PayInfo, PayInfoViewModel>(),
                    deliveryInfo = order.deliveryInfo.MappToViewModelParameter<DeliveryInfo, DeliveryInfoViewModel>(),
                    orderStatus = order.orderStatus,
                    OrderDateTime = order.OrderDateTime
                };
                ordersView.Add(orderViewModel);
            }
            return ordersView;
        }

        public static Order ConvertToOrderDb(OrderViewModel order, List<BasketItem> basketItems)
        {
            return new Order
            {
                Items = basketItems,
                payInfo = order.payInfo.MappToDbParameter<PayInfoViewModel, PayInfo>(),
                deliveryInfo = order.deliveryInfo.MappToDbParameter<DeliveryInfoViewModel, DeliveryInfo>(),
                orderStatus = order.orderStatus
            };
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
                Flavors = product.Flavors.MappToViewModelParameters<Flavor, FlavorViewModel>(),
                Description = product.Description,
                AmountInStock = product.AmountInStock,
                Pictures = product.Pictures,
                DiscountDescription = product.DiscountDescription ?? string.Empty,
                DiscountCost = product.DiscountCost,
                Concurrency = product.Concurrency,
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
                    Flavor = product.Flavor.MappToViewModelParameter<Flavor, FlavorViewModel>()
                };
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
        public static List<BasketItemViewModel> ConvertToBasketItemsView(List<BasketItem> items)
        {
            var basketItems = new List<BasketItemViewModel>();
            foreach (var item in items)
            {
                var newItem = new BasketItemViewModel
                {
                    Id = item.Id,
                    Product = ConvertToProductView(item.Product),
                    ProductInfo = item.ProductInfo.MappToViewModelParameter<ChoosingProductInfo, ChoosingProductInfoViewModel>(),
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
                Flavors = product.Flavors.MappToDbParameters<FlavorViewModel, Flavor>(),
                Description = product.Description,
                AmountInStock = product.AmountInStock,
                Pictures = product.Pictures,
                DiscountDescription = product.DiscountDescription ?? string.Empty,
                DiscountCost = product.DiscountCost,
                Concurrency = product.Concurrency
            };
        }
    }
}
