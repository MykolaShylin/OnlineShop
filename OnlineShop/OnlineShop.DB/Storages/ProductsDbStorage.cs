using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;
using System.Security.Cryptography;

namespace OnlineShop.DB.Storages
{
    public class ProductsDbStorage : IProductsStorage
    {
        private readonly DataBaseContext dataBaseContext;
        private List<Product> _products;
        public ProductsDbStorage(DataBaseContext dataBaseContext)
        {
            _products = new List<Product>();
            this.dataBaseContext = dataBaseContext;
        }

        public async Task SaveAsync(Product product)
        {
            var passedInTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 120 };
            var entry = dataBaseContext.Entry(product).Property(u => u.Concurrency);
            entry.OriginalValue = passedInTimestamp;

            dataBaseContext.Products.Add(product);
            await dataBaseContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Product product)
        {
            dataBaseContext.Products.Remove(product);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task EditAsync(Product product)
        {
            var updateProduct = await TryGetByIdAsync(product.Id);

            dataBaseContext.Entry(updateProduct).Property(x=>x.Concurrency).OriginalValue = product.Concurrency;
            updateProduct.Category = product.Category;
            updateProduct.Brand = product.Brand;
            updateProduct.Name = product.Name;
            updateProduct.Description = product.Description;
            updateProduct.Cost = product.Cost;
            updateProduct.AmountInStock = product.AmountInStock;
            updateProduct.Flavors.Clear();
            foreach (var flavor in product.Flavors)
            {
                if (flavor != null)
                {
                    updateProduct.Flavors.Add(flavor);
                }
            }
            updateProduct.BasketItems = product.BasketItems;
            updateProduct.Pictures = product.Pictures;
            updateProduct.DiscountCost = product.DiscountCost;
            updateProduct.DiscountDescription = product.DiscountDescription;
            
            await dataBaseContext.SaveChangesAsync();
        }
        public async Task<Product> TryGetByIdAsync(int id)
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).FirstOrDefaultAsync(prod => prod.Id == id);
        }
        public async Task<List<Product>> TryGetByCategoryAsync(ProductCategories category)
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).Where(prod => prod.Category == category).ToListAsync();
        }
        public async Task<Product> TryGetByNameAsync(string name)
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).FirstOrDefaultAsync(prod => prod.Name == name);
        }
        public async Task<List<Product>> GetAllAsync()
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).ToListAsync();
        }

        public async Task InitializeDefaultProductsAsync()
        {
            var FlavorsDb = new Flavor[]
            {
                    new Flavor { Name = "Шоколад" },
                    new Flavor { Name = "Ваниль"},
                    new Flavor { Name = "Карамель"},
                    new Flavor { Name = "Кофе" },
                    new Flavor { Name = "Кокос" },
                    new Flavor { Name = "Печенье" },
                    new Flavor { Name = "Банан" },
                    new Flavor { Name = "Клубника"},
                    new Flavor { Name = "Ананас"},
                    new Flavor {  Name = "Лимон"},
                    new Flavor {  Name = "Арбуз"},
                    new Flavor {  Name = "Без вкуса" }
            };

            dataBaseContext.Flavors.AddRange(FlavorsDb);

            var ProductsDb = new Product[]
            {
                    new Product
                    {
                        Category = ProductCategories.Protein,
                        Brand = "Optimum Nutrition",
                        Name = "100% Whey Protein Professional",
                        Cost = 2750,
                        Description = "100% Whey Protein Professional – высококачественный ультраизолированый концентрат из сывороточного белка и сывороточного изолята.100% Whey Protein Professional - имеет очень большое разнообразие вкусов, и среди них, Вы точно найдете свой любимый. Кроме этого, протеин хорошо смешивается и растворяется с водой и молоком.",
                        Pictures = new List<ProductPicture>(){new ProductPicture {Path="/prod_pictures/Protein/ON_Gold_Standart_100_Whey.png" } },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.Protein,
                        Brand = "Scitec Nutrition",
                        Name = "100% Whey Gold Standard",
                        Cost = 2500,
                        Description = "Cамый продаваемый в мире сывороточный протеин. Каждая порция Gold Standard 100% Whey содержит 24 грамма быстроусваиваемого сывороточного протеина с минимальным содержанием жира, лактозы и других ненужных веществ.",
                        Pictures = new List < ProductPicture >(){new ProductPicture {Path="/prod_pictures/Protein/SN_100_Whey_Professionall.png" } },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.Protein,
                        Brand = "Scitec Nutrition",
                        Name = "100% Whey Isolate",
                        Cost = 3500,
                        Description = "100% Whey Isolate от Scitec Nutrition, высококачественный сывороточный изолят, дополнительно обогащен гидролизатом белка. Компании Scitec, удалось создать действительно качественный продукт с высочайшей степенью очищения и великолепным вкусом, при полном отсутствии сахара, жиров и лактозы. Благодаря прохождению многоуровневой очистки, имеет болей высокое содержание белка нежели сывороточные концентрата.",
                        Pictures = new List < ProductPicture >(){new ProductPicture {Path= "/prod_pictures/Protein/SN_100_Whey_Isolate.png" } },
                        AmountInStock = 30
                    },

                    new Product
                    {
                        Category = ProductCategories.ProteinBar,
                        Brand = "Sporter",
                        Name = "Zero One",
                        Cost = 65,
                        Description = "Протеиновые батончики торговой марки Sporter - созданы для удобного и вкусного перекуса в течение дня или после изнурительной тренировки.Нежная текстура батончика и низкое содержание сахара позволяет заменить им десерт и поддерживать диету не отказывая себе в сладком.",
                        Pictures = new List < ProductPicture >(){new ProductPicture {Path ="/prod_pictures/Bars/Sporter_ZeroOne.png" } },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.BCAA,
                        Brand = "BSN",
                        Name = "AMINOx",
                        Cost = 900,
                        Description = "ВСАА поддерживают рост мышечной массы и предотвращают мышечное истощение. Именно такой эффект необходим во время периода интенсивных тренировок. Amino X марки BSN - это первые шипучие растворимые аминокислоты для выносливости и восстановления.",
                        Pictures = new List < ProductPicture >(){new ProductPicture {Path ="/prod_pictures/BCAA/BSN_AMINOx.png" } },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.Creatine,
                        Brand = "MST",
                        Name = "MST Creatine Kick",
                        Cost = 250,
                        Description = "MST® CreatineKick 7 in 1 – это многокомпонентный быстро усвояемый креатин в инновационной формуле, объединяет несколько видов креатина в одном продукте. Он соединяет свойства и преимущества различных видов креатина в одну смесь. Содержит 7 видов креатина.",
                        Pictures = new List < ProductPicture >(){new ProductPicture {Path = "/prod_pictures/creatine/MST_CREATINE_KICK.png" } },
                        AmountInStock = 25
                    },

                    new Product
                    {
                        Category = ProductCategories.Citruline,
                        Brand = "Biotech USA",
                        Name = "Citruline Malate",
                        Cost = 400,
                        Description = "Биологически активная добавка, которая в себе содержит цитруллин и яблочную кислоту – малат, усиливающую воздействие аминокислоты на организм и способствует ее усвоению. В момент, когда организм после усиленного режима тренировок нуждается в восстановлении, когда есть ощущения недостатка энергии и потери сил, то прием препарата Citrulline Malate окажет благотворное воздействие и наполнит энергией для дальнейших спортивных достижений.",
                        Pictures = new List < ProductPicture >(){ new ProductPicture {Path= "/prod_pictures/citruline/BioTechUsa_Citr_Mal.png"  } },
                        AmountInStock = 70
                    },
                    new Product
                    {
                        Category = ProductCategories.Gainer,
                        Brand = "Dymatize",
                        Name = "Super Mass Gainer",
                        Cost = 1700,
                        Description = "Dymatize Super Mass Gainer – то, что нужно для супер-компенсации мышечной системы у людей с очень быстрым метаболизмом и недобором веса. Максимальное количество быстрых углеводов и минимальное содержание протеинов позволит быстро набрать вес. Смесь содержит насыщенные жиры – они выступают строительным материалом для мужских гормонов – а вместе с ним начнут расти мышцы.",
                        Pictures = new List < ProductPicture > () { new ProductPicture {Path="/prod_pictures/geiner/Dymatize_Super_Mass.png" } },
                        AmountInStock = 60
                    },
                    new Product
                    {
                        Category = ProductCategories.Creatine,
                        Brand = "Rule1",
                        Name = "R1 Creatine",
                        Cost = 420,
                        Description = "Креатин R1 представляет собой самую популярную и эффективную добавку для повышения производительности в наиболее изученной форме (моногидрат). Креатин участвует в энергетическом обмене в мышечных и нервных клетках, поэтому дополнительный прием этой добавки особенно необходим в период интенсивных нагрузок. Более того, наш порошок моногидрата креатина микронизирован, для максимально быстрого и полного усвоения.",
                        Pictures = new List < ProductPicture >() {new ProductPicture {Path="/prod_pictures/creatine/Rule1_R1_Creatine.png" } },
                        AmountInStock = 55
                    },
            };

            dataBaseContext.Products.AddRange(ProductsDb);

            ProductsDb[0].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[1], FlavorsDb[2], FlavorsDb[3], FlavorsDb[4] });
            ProductsDb[1].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[3], FlavorsDb[4], FlavorsDb[5], FlavorsDb[6] });
            ProductsDb[2].Flavors.AddRange(new List<Flavor> { FlavorsDb[1], FlavorsDb[2], FlavorsDb[7], FlavorsDb[11] });
            ProductsDb[3].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[4], FlavorsDb[7] });
            ProductsDb[4].Flavors.AddRange(new List<Flavor> { FlavorsDb[8], FlavorsDb[9], FlavorsDb[10] });
            ProductsDb[5].Flavors.AddRange(new List<Flavor> { FlavorsDb[7], FlavorsDb[10], FlavorsDb[11] });
            ProductsDb[6].Flavors.AddRange(new List<Flavor> { FlavorsDb[7], FlavorsDb[8], FlavorsDb[9], FlavorsDb[10], FlavorsDb[11] });
            ProductsDb[7].Flavors.AddRange(new List<Flavor> { FlavorsDb[1], FlavorsDb[2], FlavorsDb[4], FlavorsDb[5], FlavorsDb[6] });
            ProductsDb[8].Flavors.AddRange(new List<Flavor> { FlavorsDb[11] });

            await dataBaseContext.SaveChangesAsync();
        }

        public async Task ClearAllAsync()
        {
            dataBaseContext.Flavors.RemoveRange(dataBaseContext.Flavors);
            dataBaseContext.Products.RemoveRange(dataBaseContext.Products);
            dataBaseContext.Pictures.RemoveRange(dataBaseContext.Pictures);
            await dataBaseContext.SaveChangesAsync();
        }
    }
}
