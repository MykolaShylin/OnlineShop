using AutoMapper;
using OnlineShop.DB.Models;
using OnlineShopWebApp.FeedbackApi;
using OnlineShopWebApp.FeedbackApi.Models;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Helpers
{
    public class MappingProfile : Profile
    {        
        public MappingProfile()
        {            

            CreateMap<Flavor, FlavorViewModel>().ReverseMap();
            CreateMap<GoogleMapShopInfo, GoogleMapShopInfoViewModel>().ReverseMap();

            CreateMap<ComparingProducts, ComparingProductsViewModel>().ReverseMap();

            CreateMap<PayInfo, PayInfoViewModel>()
                .ReverseMap()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id == 0 ? new PayInfo().Id : x.Id));

            CreateMap<DeliveryInfo, DeliveryInfoViewModel>()
                .ReverseMap()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id == 0 ? new DeliveryInfo().Id : x.Id));

            CreateMap<Feedback, FeedbackViewModel>().ReverseMap();

            CreateMap<ChoosingProductInfo, ChoosingProductInfoViewModel>()
                .ReverseMap()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id == 0 ? new ChoosingProductInfo().Id : x.Id));

            CreateMap<Role, RoleViewModel>().ReverseMap();

            CreateMap<User, UserViewModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.RealName.Split().Count() > 1 ? x.RealName.Split().First() : x.RealName))
                .ForMember(x => x.SerName, opt => opt.MapFrom(x => x.SerName))
                .ForMember(x => x.Login, opt => opt.MapFrom(x => x.UserName))
                .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber))
                .ReverseMap()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id ?? new User().Id))
                .ForPath(x => x.RealName, opt => opt.MapFrom(x => x.Name))
                .ForPath(x => x.UserName, opt => opt.MapFrom(x => x.Login))
                .ForPath(x => x.PhoneNumber, opt => opt.MapFrom(x => x.Phone));

            CreateMap<User, EditUserViewModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.RealName.Split().Count() > 1 ? x.RealName.Split().First() : x.RealName))
                .ForMember(x => x.SerName, opt => opt.MapFrom(x => x.SerName ?? x.RealName.Split().Last()))
                .ForMember(x => x.Login, opt => opt.MapFrom(x => x.UserName))
                .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber));

            CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.Flavor, opt => opt.MapFrom(x => x.Flavors.First()))
                .ForMember(x => x.DiscountDescription, opt => opt.MapFrom(x => x.DiscountDescription ?? string.Empty))
                .ForMember(x => x.Cost, opt => opt.MapFrom(x => Math.Truncate(x.Cost)))
                .ForMember(x => x.DiscountCost, opt => opt.MapFrom(x => Math.Truncate(x.DiscountCost)))
                .ReverseMap()
                .ForPath(x => x.DiscountDescription, opt => opt.MapFrom(x => x.DiscountDescription ?? string.Empty));

            CreateMap<Product, MainPageProductsViewModel>()
                .ForMember(x => x.Flavor, opt => opt.MapFrom(x => x.Flavors.First()))
                .ForMember(x => x.Cost, opt => opt.MapFrom(x => Math.Truncate(x.Cost)))
                .ForMember(x => x.DiscountCost, opt => opt.MapFrom(x => Math.Truncate(x.DiscountCost)))
                .ReverseMap();

            CreateMap<EditBasket, EditBasketViewModel>().ReverseMap();

            CreateMap<Discount, DiscountViewModel>().ReverseMap();

            CreateMap<BasketItem, BasketItemViewModel>().ReverseMap();

            CreateMap<Basket, BasketViewModel>().ReverseMap();

            CreateMap<FavoriteProduct, FavoriteProductViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>()
                .AfterMap((src, dest) => dest.deliveryInfo.CustomerId = dest.deliveryInfo.CustomerId.ToString().Substring(0, 10))
                .ReverseMap();            
        }
    }
}
