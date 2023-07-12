using Azure;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Contexts
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ComparingProducts> ComparingProducts { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Flavor> Flavors { get; set; }
        public DbSet<Order> ClosedOrders { get; set; }
        public DbSet<ProductPicture> Pictures { get; set; }
        public DbSet<Discount> Discounts { get; set; }


        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().Property(x => x.Concurrency)
            .IsConcurrencyToken(true)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnName("Concurrency");

            modelBuilder.Entity<Product>().HasMany(x=>x.Pictures).WithOne().OnDelete(DeleteBehavior.Cascade);

            //var DiscountsDb = new Discount[]
            //{
            //    new Discount() { Id = 1, DiscountPercent= 5 },
            //    new Discount() { Id = 2, DiscountPercent= 10},
            //    new Discount() { Id = 3, DiscountPercent= 15 },
            //    new Discount() { Id = 4, DiscountPercent= 20 },
            //    new Discount() { Id = 5, DiscountPercent= 25 },
            //    new Discount() { Id = 6, DiscountPercent= 30 },
            //    new Discount() { Id = 7, DiscountPercent= 50 },
            //    new Discount() { Id = 8, DiscountPercent= 80 },
            //};

            //modelBuilder.Entity<Discount>().HasData(DiscountsDb);

            //var FlavorsDb = new Flavor[]
            //{
            //    new Flavor { Id = 1, Name = "Шоколад" },
            //    new Flavor { Id = 2, Name = "Ваниль"},
            //    new Flavor { Id = 3, Name = "Карамель"},
            //    new Flavor { Id = 4, Name = "Кофе" },
            //    new Flavor { Id = 5, Name = "Кокос" },
            //    new Flavor { Id = 6, Name = "Печенье" },
            //    new Flavor { Id = 7, Name = "Банан" },
            //    new Flavor { Id = 8, Name = "Клубника"},
            //    new Flavor { Id = 9, Name = "Ананас"},
            //    new Flavor { Id = 10, Name = "Лимон"},
            //    new Flavor { Id = 11, Name = "Арбуз"},
            //    new Flavor { Id = 12, Name = "Без вкуса" }
            //};

            //var ProductsDb = new Product[]
            //{
            //    new Product
            //    {
            //        Id = 1,
            //        Category = ProductCategories.Protein,
            //        Brand = "Optimum Nutrition",
            //        Name = "100% Whey Protein Professional",
            //        Cost = 2750,
            //        Description = "100% Whey Protein Professional – высококачественный ультраизолированый концентрат из сывороточного белка и сывороточного изолята.100% Whey Protein Professional - имеет очень большое разнообразие вкусов, и среди них, Вы точно найдете свой любимый. Кроме этого, протеин хорошо смешивается и растворяется с водой и молоком.",
            //        Pictures = "/prod_pictures/whey_protein/ON Gold Standart.png",
            //        AmountInStock = 100

            //    },

            //    new Product
            //    {
            //        Id = 2,
            //        Category = ProductCategories.Protein,
            //        Brand = "Scitec Nutrition",
            //        Name = "100% Whey Gold Standard",
            //        Cost = 2500,
            //        Description = "Cамый продаваемый в мире сывороточный протеин. Каждая порция Gold Standard 100% Whey содержит 24 грамма быстроусваиваемого сывороточного протеина с минимальным содержанием жира, лактозы и других ненужных веществ.",
            //        Pictures = "/prod_pictures/whey_protein/SN Professionall.png",
            //        AmountInStock = 100
            //    },

            //    new Product
            //    {
            //        Id = 3,
            //        Category = ProductCategories.Protein,
            //        Brand = "Scitec Nutrition",
            //        Name = "100% Whey Isolate",
            //        Cost = 3500,
            //        Description = "100% Whey Isolate от Scitec Nutrition, высококачественный сывороточный изолят, дополнительно обогащен гидролизатом белка. Компании Scitec, удалось создать действительно качественный продукт с высочайшей степенью очищения и великолепным вкусом, при полном отсутствии сахара, жиров и лактозы. Благодаря прохождению многоуровневой очистки, имеет болей высокое содержание белка нежели сывороточные концентрата.",
            //        Pictures = "/prod_pictures/isolate_protein/SN-100-Whey-Isolate.png",
            //        AmountInStock = 30
            //    },

            //    new Product
            //    {
            //        Id = 4,
            //        Category = ProductCategories.ProteinBar,
            //        Brand = "Sporter",
            //        Name = "Zero One",
            //        Cost = 65,
            //        Description = "Протеиновые батончики торговой марки Sporter - созданы для удобного и вкусного перекуса в течение дня или после изнурительной тренировки.Нежная текстура батончика и низкое содержание сахара позволяет заменить им десерт и поддерживать диету не отказывая себе в сладком.",
            //        Pictures = "/prod_pictures/Bars/Sporter ZeroOne.png",
            //        AmountInStock = 100
            //    },

            //    new Product
            //    {
            //        Id = 5,
            //        Category = ProductCategories.BCAA,
            //        Brand = "BSN",
            //        Name = "AMINOx",
            //        Cost = 900,
            //        Description = "ВСАА поддерживают рост мышечной массы и предотвращают мышечное истощение. Именно такой эффект необходим во время периода интенсивных тренировок. Amino X марки BSN - это первые шипучие растворимые аминокислоты для выносливости и восстановления.",
            //        Pictures = "/prod_pictures/BCAA/BSNx.png",
            //        AmountInStock = 100
            //    },

            //    new Product
            //    {
            //        Id = 6,
            //        Category = ProductCategories.Creatine,
            //        Brand = "MST",
            //        Name = "MST Creatine Kick",
            //        Cost = 250,
            //        Description = "MST® CreatineKick 7 in 1 – это многокомпонентный быстро усвояемый креатин в инновационной формуле, объединяет несколько видов креатина в одном продукте. Он соединяет свойства и преимущества различных видов креатина в одну смесь. Содержит 7 видов креатина.",
            //        Pictures = "/prod_pictures/creatine/MST-CREATINE-KICK.png",
            //        AmountInStock = 25
            //    },

            //    new Product
            //    {
            //        Id = 7,
            //        Category = ProductCategories.Citruline,
            //        Brand = "Biotech USA",
            //        Name = "Citruline Malate",
            //        Cost = 400,
            //        Description = "Биологически активная добавка, которая в себе содержит цитруллин и яблочную кислоту – малат, усиливающую воздействие аминокислоты на организм и способствует ее усвоению. В момент, когда организм после усиленного режима тренировок нуждается в восстановлении, когда есть ощущения недостатка энергии и потери сил, то прием препарата Citrulline Malate окажет благотворное воздействие и наполнит энергией для дальнейших спортивных достижений.",
            //        Pictures = "/prod_pictures/citruline/BioTechUsa_Citr_mal.png",
            //        AmountInStock = 70,
            //    },

            //    new Product
            //    {
            //        Id = 8,
            //        Category = ProductCategories.Gainer,
            //        Brand = "Dymatize",
            //        Name = "Super Mass Gainer",
            //        Cost = 1700,
            //        Description = "Dymatize Super Mass Gainer – то, что нужно для супер-компенсации мышечной системы у людей с очень быстрым метаболизмом и недобором веса. Максимальное количество быстрых углеводов и минимальное содержание протеинов позволит быстро набрать вес. Смесь содержит насыщенные жиры – они выступают строительным материалом для мужских гормонов – а вместе с ним начнут расти мышцы.",
            //        Pictures = "/prod_pictures/geiner/Dymatize_Super_Mass.png",
            //        AmountInStock = 60
            //    }
            //};

            //modelBuilder.Entity<Flavor>().HasData(FlavorsDb);
            //modelBuilder.Entity<Product>().HasData(ProductsDb);

            //modelBuilder.Entity<Product>().HasMany(e => e.Flavors).WithMany(e => e.Products).UsingEntity<Dictionary<string, object>>("ProductFlavor",
            //    x => x.HasOne<Flavor>().WithMany().HasForeignKey("FlavorId"),
            //    x => x.HasOne<Product>().WithMany().HasForeignKey("ProductId"));

        }
    }

}
