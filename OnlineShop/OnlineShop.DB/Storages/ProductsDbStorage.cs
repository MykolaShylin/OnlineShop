using Microsoft.Ajax.Utilities;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Migrations;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;
using System.Security.Cryptography;

namespace OnlineShop.DB.Storages
{
    public class ProductsDbStorage : IProductsStorage
    {
        private readonly DataBaseContext dataBaseContext;
        public ProductsDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }

        public async Task SaveAsync(Product product)
        {
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

            dataBaseContext.Entry(updateProduct).Property(x => x.Concurrency).OriginalValue = product.Concurrency;
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
        public async Task<List<Product>> TryGetByCategoryAsync(ProductCategories categorie)
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).Where(x=> categorie == x.Category).ToListAsync();
        }

        public async Task<List<Product>> TryGetByBrandAsync(ProductBrands brand)
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).Where(prod => prod.Brand == brand).ToListAsync();
        }
        public async Task<Product> TryGetByNameAsync(string name)
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).FirstOrDefaultAsync(prod => prod.Name == name);
        }
        public async Task<List<Product>> GetAllAsync()
        {
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).ToListAsync();
            
        }

        public async Task ClearAllAsync()
        {
            dataBaseContext.Flavors.RemoveRange(dataBaseContext.Flavors);
            dataBaseContext.Products.RemoveRange(dataBaseContext.Products);
            dataBaseContext.Pictures.RemoveRange(dataBaseContext.Pictures);
            dataBaseContext.Discounts.RemoveRange(dataBaseContext.Discounts);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task ReduceAmountInStock(List<BasketItem> items)
        {
            foreach (var item in items)
            {
                var product = await TryGetByIdAsync(item.Product.Id);
                product.AmountInStock = item.Amount > product.AmountInStock ? 0 : product.AmountInStock - item.Amount;
            }
            dataBaseContext.SaveChanges();
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
                    new Flavor {  Name = "Без вкуса" },
                    new Flavor {  Name = "Печенье-крем" }
            };

            dataBaseContext.Flavors.AddRange(FlavorsDb);

            var ProductsDb = new Product[]
            {
                    new Product
                    {
                        Category = ProductCategories.Protein,
                        Brand = ProductBrands.Optimum_Nutrition,
                        Name = "100% Whey Gold Standard",
                        Cost = 2750,
                        Description = "Cамый продаваемый в мире сывороточный протеин. Каждая порция Gold Standard 100% Whey содержит 24 грамма быстроусваиваемого сывороточного протеина с минимальным содержанием жира, лактозы и других ненужных веществ.",
                        Pictures = new List<ProductPicture>()
                        {
                            new ProductPicture
                            { 
                                Path="/prod_pictures/Protein/ON_Gold_Standart_100_Whey.png",
                                NutritionPath = "/prod_pictures/Nutritions/ON-Whey-Gold-Standard-nutr.jpg"
                            } 
                        },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.Protein,
                        Brand = ProductBrands.Scitec_Nutrition,
                        Name = "100% Whey Protein Professional",
                        Cost = 2500,
                        Description = "100% Whey Protein Professional – высококачественный ультраизолированый концентрат из сывороточного белка и сывороточного изолята.100% Whey Protein Professional - имеет очень большое разнообразие вкусов, и среди них, Вы точно найдете свой любимый. Кроме этого, протеин хорошо смешивается и растворяется с водой и молоком.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture 
                            {
                                Path="/prod_pictures/Protein/SN_100_Whey_Professionall.png",
                                NutritionPath = "/prod_pictures/Nutritions/SN-Whey-Protein-Professional-nutr.jpg"
                            }
                        },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.Protein,
                        Brand = ProductBrands.Scitec_Nutrition,
                        Name = "100% Whey Isolate",
                        Cost = 3500,
                        Description = "100% Whey Isolate от Scitec Nutrition, высококачественный сывороточный изолят, дополнительно обогащен гидролизатом белка. Компании Scitec, удалось создать действительно качественный продукт с высочайшей степенью очищения и великолепным вкусом, при полном отсутствии сахара, жиров и лактозы. Благодаря прохождению многоуровневой очистки, имеет болей высокое содержание белка нежели сывороточные концентрата.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture 
                            {
                                Path= "/prod_pictures/Protein/SN_100_Whey_Isolate.png",
                                NutritionPath = "/prod_pictures/Nutritions/SN-Whey-Isolate-nutr.jpg"
                            }
                        },
                        AmountInStock = 30
                    },

                    new Product
                    {
                        Category = ProductCategories.ProteinBar,
                        Brand = ProductBrands.Sporter,
                        Name = "Zero One",
                        Cost = 65,
                        Description = "Протеиновые батончики торговой марки Sporter - созданы для удобного и вкусного перекуса в течение дня или после изнурительной тренировки.Нежная текстура батончика и низкое содержание сахара позволяет заменить им десерт и поддерживать диету не отказывая себе в сладком.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path ="/prod_pictures/ProteinBar/Sporter_ZeroOne.png",
                                NutritionPath = "/prod_pictures/Nutritions/Sporter-Zero-One-Nutr.png"
                            } 
                        },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.BCAA,
                        Brand = ProductBrands.BSN,
                        Name = "AMINOx",
                        Cost = 900,
                        Description = "ВСАА поддерживают рост мышечной массы и предотвращают мышечное истощение. Именно такой эффект необходим во время периода интенсивных тренировок. Amino X марки BSN - это первые шипучие растворимые аминокислоты для выносливости и восстановления.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture 
                            {
                                Path ="/prod_pictures/BCAA/BSN_AMINOx.png",
                                NutritionPath = "/prod_pictures/Nutritions/bsn-amino-x-nutr.jpg"
                            } 
                        },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.Creatine,
                        Brand = ProductBrands.MST,
                        Name = "MST Creatine Kick",
                        Cost = 250,
                        Description = "MST® CreatineKick 7 in 1 – это многокомпонентный быстро усвояемый креатин в инновационной формуле, объединяет несколько видов креатина в одном продукте. Он соединяет свойства и преимущества различных видов креатина в одну смесь. Содержит 7 видов креатина.",
                        Pictures = new List < ProductPicture >()                        
                        {
                            new ProductPicture 
                            {
                                Path = "/prod_pictures/creatine/MST_CREATINE_KICK.png",
                                NutritionPath = "/prod_pictures/Nutritions/MST-Kreatine-Kick-nutr.jpg"
                            }
                        },
                        AmountInStock = 25
                    },

                    new Product
                    {
                        Category = ProductCategories.Citruline,
                        Brand = ProductBrands.Biotech_USA,
                        Name = "Citruline Malate",
                        Cost = 400,
                        Description = "Биологически активная добавка, которая в себе содержит цитруллин и яблочную кислоту – малат, усиливающую воздействие аминокислоты на организм и способствует ее усвоению. В момент, когда организм после усиленного режима тренировок нуждается в восстановлении, когда есть ощущения недостатка энергии и потери сил, то прием препарата Citrulline Malate окажет благотворное воздействие и наполнит энергией для дальнейших спортивных достижений.",
                        Pictures = new List < ProductPicture >()
                        { 
                            new ProductPicture 
                            {
                                Path= "/prod_pictures/citruline/BioTechUsa_Citr_Mal.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/BioTechUSA-Citruline-Malate-nutr.jpg"
                            }
                        },
                        AmountInStock = 70
                    },
                    new Product
                    {
                        Category = ProductCategories.Gainer,
                        Brand = ProductBrands.Dymatize,
                        Name = "Super Mass Gainer",
                        Cost = 1700,
                        Description = "Dymatize Super Mass Gainer – то, что нужно для супер-компенсации мышечной системы у людей с очень быстрым метаболизмом и недобором веса. Максимальное количество быстрых углеводов и минимальное содержание протеинов позволит быстро набрать вес. Смесь содержит насыщенные жиры – они выступают строительным материалом для мужских гормонов – а вместе с ним начнут расти мышцы.",
                        Pictures = new List < ProductPicture > () 
                        { 
                            new ProductPicture 
                            {
                                Path="/prod_pictures/geiner/Dymatize_Super_Mass.png",
                                NutritionPath = "/prod_pictures/Nutritions/Dymatize-Super-Mass-nutr.jpeg"
                            } 
                        },
                        AmountInStock = 60
                    },
                    new Product
                    {
                        Category = ProductCategories.Creatine,
                        Brand = ProductBrands.Rule1,
                        Name = "R1 Creatine",
                        Cost = 420,
                        Description = "Креатин R1 представляет собой самую популярную и эффективную добавку для повышения производительности в наиболее изученной форме (моногидрат). Креатин участвует в энергетическом обмене в мышечных и нервных клетках, поэтому дополнительный прием этой добавки особенно необходим в период интенсивных нагрузок. Более того, наш порошок моногидрата креатина микронизирован, для максимально быстрого и полного усвоения.",
                        Pictures = new List < ProductPicture >() 
                        {
                            new ProductPicture 
                            {
                                Path="/prod_pictures/creatine/Rule1_R1_Creatine.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/rule-1-creatine-nutr.jpg"
                            } 
                        },
                        AmountInStock = 55
                    },
                    new Product
                    {
                        Category = ProductCategories.FatBurner,
                        Brand = ProductBrands.TREC,
                        Name = "Gold Core Line Clenburexin",
                        Cost = 600,
                        Description = "Trec Nutrition Gold Core Line Clenburexin - тщательно подобранная комбинация натуральных растительных термогеников, витаминов и кофеина. Обнаруживает термогенноеокисление жиров + увеличивает экскрецию воды из организма, и, как вы знаете: удержание воды является одной из основных причин увеличения индекса массы тела. Формула Trec Nutrition Gold Core Line Clenburexin основана на растительных экстрактах, которые «разгоняют» наш метаболизм и стимулируют действие. Формула препарата оказывает большое влияние на регуляцию аппетита в течение дня, способствуя тем самым поддержанию дефицита калорий при снижении веса.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/FatBurner/TREC-Fat-Burner.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/TREC-Clenburexin-nutr.png"
                            }
                        },
                        AmountInStock = 120
                    },
                    new Product
                    {
                        Category = ProductCategories.ProteinBar,
                        Brand = ProductBrands.GO_ON,
                        Name = "Protein Bar 50%",
                        Cost = 55,
                        Description = "GO ON NUTRITION Protein Bar 50% - протеиновый батончик со вкусом сливок и печенья с добавлением хлопьев какао. Он обеспечивает целых 20 г белка и всего 161 ккал, поэтому является отличной альтернативой калорийным вредным сладостям. Отличается высоким содержанием белка, который способствует наращиванию мышечной массы и ускоряет регенерацию после тренировки. Будет отличным и вкусным перекусом до или после тренировки как ценный источник энергии. Идеальный выбор для людей, которые заботятся о правильном количестве потребляемого белка, для спортсменов и людей, ведущих активный образ жизни.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/ProteinBar/GO_ON_Prot50.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/GO-ON-Protein-Bar-50%-nutr.png"
                            }
                        },
                        AmountInStock = 90
                    },
                    new Product
                    {
                        Category = ProductCategories.BCAA,
                        Brand = ProductBrands.Biotech_USA,
                        Name = "BCAA Flash ZERO",
                        Cost = 1000,
                        Description = "BioTech (USA) BCAA Flash ZERO –  уникальная пищевая добавка для профессиональных бодибилдеров и новичков, призванная обеспечить прирост мускулатуры и ее правильное развитие, снизить катаболические проявления. В основе продукта – не только аминокислоты разветвленной цепи ВСАА фармацевтического качества, но и глютамин и витамин В6, необходимые для роста мускулов, здоровья соединительной ткани и опорно-двигательного аппарата.\r\n\r\nПри регулярном употреблении – продукт активизирует метаболические процессы, налаживает нормальную гидратацию тканей, облегчает процесс потери веса процесс похудения. Стимулирует регенерацию хрящевой ткани. ",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/BCAA/BioTechUSA-BCAA-Zero.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/bcaa-flash-biotech-nutr.jpg"
                            }
                        },
                        AmountInStock = 90
                    },
                    new Product
                    {
                        Category = ProductCategories.Caseine,
                        Brand = ProductBrands.Dymatize,
                        Name = "Elite Casein Protein Powder",
                        Cost = 2400,
                        Description = "Казеин — это уникальный протеин, получаемый из молока, который достаточно медленно расщепляется в системе пищеварения, что помогает улучшить усваиваемость и обеспечить медленное высвобождение используемых для роста мышц аминокислот. Поэтому если ваша цель — нарастить мышцы или предотвратить распад мышечного протеина в промежутках между приемами пищи или во время сна, то Dymatize Elite Casein Protein Powder является отличным выбором.\r\n\r\nПри производстве комплекса Elite Casein используется процесс перекрестной микрофильтрации, который помогает сохранить натуральное состояние казеина, важного протеина, участвующего в процессе роста мышечной массы.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/Caseint/Dymatize_Elite_Casein.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/Dymatize-Elite-Casein-nutr.jpg"
                            }
                        },
                        AmountInStock = 75
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
            ProductsDb[9].Flavors.AddRange(new List<Flavor> { FlavorsDb[11] });
            ProductsDb[10].Flavors.AddRange(new List<Flavor> { FlavorsDb[12] });
            ProductsDb[11].Flavors.AddRange(new List<Flavor> { FlavorsDb[8], FlavorsDb[9], FlavorsDb[11] });
            ProductsDb[12].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[1], FlavorsDb[2], FlavorsDb[3] });

            var DiscountsDb = new Discount[]
            {
                new Discount() { DiscountPercent= 0 },
                new Discount() { DiscountPercent= 5 },
                new Discount() { DiscountPercent= 10 },
                new Discount() { DiscountPercent= 15 },
                new Discount() { DiscountPercent= 20 },
                new Discount() { DiscountPercent= 25 },
                new Discount() { DiscountPercent= 30 },
                new Discount() { DiscountPercent= 50 },
                new Discount() { DiscountPercent= 80 }
            };

            dataBaseContext.Discounts.AddRange(DiscountsDb);

            DiscountsDb[0].Products.AddRange(ProductsDb);

            DiscountsDb[0].Products.ForEach(x => x.DiscountCost = (decimal.Ceiling(x.Cost * 100) * (100 - DiscountsDb[0].DiscountPercent) / 100) / 100);


            await dataBaseContext.Discounts.ForEachAsync(x=>x.Products.ForEach(q => q.DiscountCost = decimal.Ceiling(((q.Cost * 100) * (100 - x.DiscountPercent) / 100) / 100)));

            await dataBaseContext.SaveChangesAsync();
        }        
    }
}
