using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.DB;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Patterns;
using OnlineShop.DB.Storages;
using OnlineShop.DB.Storages.BasketStorage;
using OnlineShop.DB.Storages.ProductStorage;
using OnlineShopWebApp.FeedbackApi;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Services;
using ReturnTrue.AspNetCore.Identity.Anonymous;
using Serilog;

namespace OnlineShopWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("online_shop");
            services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(connection));
            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<User, Role>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();

            services.AddScoped<EmailService>();
            services.AddScoped<IPictures, PicturesDbStorage>();
            services.AddScoped<IProductsStorage, ProxyProductsDbStorage>();
            services.AddScoped<IFlavor, FlavorsDbStorage>();
            services.AddScoped<IPurchases, ClosedPurchasesDbStorage>();
            services.AddScoped<IBasketStorage, ProxyBasketDbStorage>();
            services.AddScoped<IProductComparer, ComparingProductsDbStorage>();
            services.AddScoped<IProductComparer, ComparingProductsDbStorage>();
            services.AddScoped<IDiscount, DiscountsDbStorage>();
            services.AddScoped<IGoogleMap, ShopContactsDbStorage>();
            services.AddScoped<IFavorite, FavoriteProductsDbStorage>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddHttpClient("FeedbackApi", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://localhost:7274");
            });
            services.AddScoped<FeedbackApiClient>();

            services.ConfigureApplicationCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.LoginPath = new PathString("/UserEntering/Login");
                    options.LogoutPath = new PathString("/ UserEntering/Logout");
                    options.Cookie = new CookieBuilder
                    {
                        IsEssential = true
                    };
                })
                .AddAuthentication()
                .AddGitHub(options =>
                {
                    options.ClientId = "8676a7efd9a7510e6b31";
                    options.ClientSecret = "a055a0b307f3db4c7ad987c95a719d1647b95d54";
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });


            services.AddControllersWithViews();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseAnonymousId(new AnonymousIdCookieOptionsBuilder()
                .SetCustomCookieTimeout(1800));

            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "MyArea",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
