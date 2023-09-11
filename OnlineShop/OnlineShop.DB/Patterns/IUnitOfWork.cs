using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Patterns
{
    public interface IUnitOfWork : IDisposable
    {
        IProductsStorage ProxyProductsDbStorage { get; }
        IBasketStorage ProxyBasketDbStorage { get; }
        IPurchases ClosedPurchasesDbStorage { get; }
        IProductComparer ComparingProductsDbStorage { get; }
        IFavorite FavoriteProductsDbStorage { get; }
        IFlavor FlavorsDbStorage { get; }
        IPictures PicturesDbStorage { get; }
        IDiscount DiscountsDbStorage { get; }        
        Task SaveChangesAsync();
    }
}
