﻿using OnlineShop.DB.Contexts;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Storages;
using Microsoft.AspNetCore.Identity;
using OnlineShop.DB.Storages.BasketStorage;
using OnlineShop.DB.Storages.ProductStorage;

namespace OnlineShop.DB.Patterns
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManager<User> _userManager;
        private readonly DataBaseContext _dataBaseContext;
        private IProductsStorage _productsDbStorage;
        private IBasketStorage _proxyBasketDbStorage;
        private IPurchases _closedPurchasesDbStorage;
        private IProductComparer _comparingProductsDbStorage;
        private IFavorite _favoriteProductsDbStorage;
        private IFlavor _flavorsDbStorage;
        private IPictures _picturesDbStorage;
        private IDiscount _discountsDbStorage;
        public UnitOfWork(DataBaseContext dataBaseContext, UserManager<User> userManager)
        {
            _dataBaseContext = dataBaseContext;
            _userManager = userManager;
        }
        public IProductsStorage ProxyProductsDbStorage
        {
            get
            {
                if (_productsDbStorage == null)
                    _productsDbStorage = new ProxyProductsDbStorage(_dataBaseContext);
                return _productsDbStorage;
            }
        }

        public IBasketStorage ProxyBasketDbStorage
        {
            get
            {
                if (_proxyBasketDbStorage == null)
                    _proxyBasketDbStorage = new ProxyBasketDbStorage(_dataBaseContext);
                return _proxyBasketDbStorage;
            }
        }

        public IPurchases ClosedPurchasesDbStorage
        {
            get
            {
                if (_closedPurchasesDbStorage == null)
                    _closedPurchasesDbStorage = new ClosedPurchasesDbStorage(_dataBaseContext, _userManager);
                return _closedPurchasesDbStorage;
            }
        }

        public IProductComparer ComparingProductsDbStorage
        {
            get
            {
                if (_comparingProductsDbStorage == null)
                    _comparingProductsDbStorage = new ComparingProductsDbStorage(_dataBaseContext);
                return _comparingProductsDbStorage;
            }
        }

        public IFavorite FavoriteProductsDbStorage
        {
            get
            {
                if (_favoriteProductsDbStorage == null)
                    _favoriteProductsDbStorage = new FavoriteProductsDbStorage(_dataBaseContext);
                return _favoriteProductsDbStorage;
            }
        }

        public IFlavor FlavorsDbStorage
        {
            get
            {
                if (_flavorsDbStorage == null)
                    _flavorsDbStorage = new FlavorsDbStorage(_dataBaseContext);
                return _flavorsDbStorage;
            }
        }

        public IPictures PicturesDbStorage
        {
            get
            {
                if (_picturesDbStorage == null)
                    _picturesDbStorage = new PicturesDbStorage(_dataBaseContext);
                return _picturesDbStorage;
            }
        }

        public IDiscount DiscountsDbStorage
        {
            get
            {
                if (_discountsDbStorage == null)
                    _discountsDbStorage = new DiscountsDbStorage(_dataBaseContext);
                return _discountsDbStorage;
            }
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dataBaseContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveChangesAsync()
        {
            await _dataBaseContext.SaveChangesAsync();
        }
    }
}
