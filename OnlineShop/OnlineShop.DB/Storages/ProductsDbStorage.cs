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
            return await dataBaseContext.Products.Include(x => x.Flavors).Include(x => x.Pictures).Where(x => categorie == x.Category).ToListAsync();
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
            var SportProductsDb = new Product[]
            {
                    new Product
                    {
                        Category = ProductCategories.Sport_Protein,
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
                        Category = ProductCategories.Sport_Protein,
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
                        Category = ProductCategories.Sport_Protein,
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
                        Category = ProductCategories.Sport_ProteinBar,
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
                        Category = ProductCategories.Sport_BCAA,
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
                        Category = ProductCategories.Sport_Creatine,
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
                        Category = ProductCategories.Sport_Citruline,
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
                        Category = ProductCategories.Sport_Gainer,
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
                        Category = ProductCategories.Sport_Creatine,
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
                        Category = ProductCategories.Sport_FatBurner,
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
                        Category = ProductCategories.Sport_ProteinBar,
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
                        Category = ProductCategories.Sport_BCAA,
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
                        Category = ProductCategories.Sport_Caseine,
                        Brand = ProductBrands.Dymatize,
                        Name = "Elite Casein Protein Powder",
                        Cost = 2400,
                        Description = "Казеин — это уникальный протеин, получаемый из молока, который достаточно медленно расщепляется в системе пищеварения, что помогает улучшить усваиваемость и обеспечить медленное высвобождение используемых для роста мышц аминокислот. Поэтому если ваша цель — нарастить мышцы или предотвратить распад мышечного протеина в промежутках между приемами пищи или во время сна, то Dymatize Elite Casein Protein Powder является отличным выбором.\r\n\r\nПри производстве комплекса Elite Casein используется процесс перекрестной микрофильтрации, который помогает сохранить натуральное состояние казеина, важного протеина, участвующего в процессе роста мышечной массы.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/Caseine/Dymatize_Elite_Casein.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/Dymatize-Elite-Casein-nutr.jpg"
                            }
                        },
                        AmountInStock = 75
                    },
                    new Product
                    {
                        Category = ProductCategories.Sport_Citruline,
                        Brand = ProductBrands.ALL_Nutrition,
                        Name = "ALLNUTRITION CITRULLINE",
                        Cost = 350,
                        Description = "ALLNUTRITION CITRULLINE - это сочетание аминокислоты L-цитруллин и малата (яблочной кислоты). Цитруллин малат помогает поддерживать уровень АТФ, уменьшая избыток лактатов для лучшего восстановления мышц после тренировки.\r\nCITRULLINE - это эндогенная аминокислота, которая не строит белков, образуется в организме человека из орнитина. Цитруллин естественным образом содержится в пищевых продуктах, например арбузы. Соответствующая доступность цитруллина необходима для правильного течения цикла мочевины, а ее эффективное течение обеспечивает устранение побочного продукта метаболизма аминокислот - аммиака, что имеет дополнительное значение при диетах с высоким содержанием белка.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/Citruline/AllNutrition_Citrulline.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/AllNutrition-citruline-fact.jpg"
                            }
                        },
                        AmountInStock = 75
                    },
                    new Product
                    {
                        Category = ProductCategories.Sport_ProteinBar,
                        Brand = ProductBrands.Nutrend,
                        Name = "Excelent",
                        Cost = 90,
                        Description = "Протеиновый батончик Excelent Protein Bar по праву считается одним из лучших вариантов перекуса для людей, ведущих активный образ жизни. Он содержит 24 % белка и 43 % углеводов, а также включает в себя L-глютамин, полезные вещества и аминокислоты, которые поддерживают рост и быстрое восстановление мышц после физических нагрузок. Для любителей быстрых перекусов - это отличная возможность побаловать себя вкусным и одновременно полезным лакомством. Протеиновые батончики Excelent Protein Bar имеют большое разнообразие вкусов, что позволяет каждому атлету выбрать вариант себе по душе.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/ProteinBar/Nutred_Exelent.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/Nutrend-Excelent-fact.jpg"
                            }
                        },
                        AmountInStock = 100
                    },
                    new Product
                    {
                        Category = ProductCategories.Sport_FatBurner,
                        Brand = ProductBrands.BPI,
                        Name = "KETO TEA Fat Burner",
                        Cost = 700,
                        Description = "Keto Tea - это смесь экзогенных солей, триглицеридов со средней длиной цепи (MCT) и экстракт чая, разработанная для поддержки кетогенной или низкоуглеводной диеты. \r\nКето-чай не только помогает тем, кто придерживается кетогенной или низкоуглеводной диеты сжигать жир для получения энергии, но и поддерживает программы детоксикации, повышает энергию и концентрацию.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/FatBurner/bpi-KETO-TEA.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/bpi-KETO-TEA-fact.jpg"
                            }
                        },
                        AmountInStock = 55
                    },
                    new Product
                    {
                        Category = ProductCategories.Sport_Aminoacid,
                        Brand = ProductBrands.Olimp,
                        Name = "Anabolic Amino",
                        Cost = 1100,
                        Description = "Anabolic Amino 5500 – уникальный аминокислотный комплекс, содержащий полный набор аминок. Если вы ведете активный и здоровый образ жизни, то эта добавка создана для вас. Узнайте о необходимости использования этого продукта спортсменами.\r\nВ состав Анаболик Амино 5500 входят все аминокислотные соединения, необходимые организму, в том числе и ВСАА. Кроме этого добавка содержит витамины, и пептиды Anabolic Amino 5500 является отличным инструментом для набора массы и ускорения восстановления.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/Aminoacid/Olimp_Anabolic_Amino.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/Olimp-Anabolic-Amino-fact.jpg"
                            }
                        },
                        AmountInStock = 80
                    },
                    new Product
                    {
                        Category = ProductCategories.Sport_Aminoacid,
                        Brand = ProductBrands.REDCON,
                        Name = "Grunt EAAs",
                        Cost = 1000,
                        Description = "Grunt EAAs - это спортивная добавка от качественного бренда Redcon1!\r\nGrunt разработан для спортсменов с любым набором навыков, желающих получить поддержку в восстановлении.\r\nGrunt® — это мощная и эффективная формула EAA, содержащая 9 аминокислот, необходимых для роста новой и сохранения существующей мышечной массы.\r\nEAA являются полноценным источником белка, а BCAA — нет.\r\nИсследования показывают, что EAA лучше всего использовать как часть вашей предтренировочной программы, в то время как BCAA наиболее полезны после тренировки из-за их более высоких порций.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/Aminoacid/Redcon1_Grunt.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/REDCON-Grunt-fact.jpg"
                            }
                        },
                        AmountInStock = 80
                    },
            };

            var HealthyFoodProductsDb = new Product[]
            {
                    new Product
                    {
                        Category = ProductCategories.HealthyFood_Peanut_Butter,
                        Brand = ProductBrands.GO_ON,
                        Name = "Peanut Butter",
                        Cost = 280,
                        Description = "GoOn Peanut вutter crunchy 100% 900 г (стекло)-это высококачественная арахисовая паста от польского производителя спортивного питания GoOn. Паста изготовлена из высококачественного арахиса с соблюдением всех европейских стандартов качества без добавления сахаров и пальмового масла. В связи с отсутствием в составе и сахаров и дополнительной соли-чистый вкус арахиса можно сочетать со сладкими, солеными или острыми блюдами.\r\nАрахисовая паста является великолепным источником белка, клетчатки, ненасыщенных жирных кислот и Омега-6 и Омега-9, а также витаминов и минералов таких как витамин Е, фолиевая кислота, ниацин, железо, фосфора, магния, калия. Идеально подходит как для спортсменов, так и для людей ведущих здоровый образ жизни.\r\nУглеводы содержащиеся в арахисовой пасте имеют один из самых низких гликемических индексов в десятки раз ниже меда, например, что защищает наш организм от развития диабета и лишних калорий.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/PeanutButter/GO_ON_PeanutButter.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/GO-ON-Nutrition-PeanutButter-fact.png"
                            }
                        },
                        AmountInStock = 100
                    },

                    new Product
                    {
                        Category = ProductCategories.HealthyFood_Peanut_Butter,
                        Brand = ProductBrands.TREC,
                        Name = "Peanut Butter Whey",
                        Cost = 440,
                        Description = "PEANUT BUTTER WHEY 100 — вкусный протеиновый крем из арахиса и концентрата сывороточного протеина WHEY 100. PEANUT BUTTER WHEY 100 содержит полезный белок, полученный из концентрата белка молочной сыворотки. Белковые сливки позволяют обеспечить белок в рационе в виде вкусных сливок.\r\nPEANUT BUTTER WHEY 100 – отличное дополнение к различным видам блюд: омлетам, блинчикам, рису, кашам, пирожным и т. д. Этот вкусный крем подслащен мальтитом. Употребление в пищу продуктов, содержащих мальтит вместо сахара, помогает поддерживать минерализацию зубов.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/PeanutButter/Trec-PeanutButter.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/GO-ON-Nutrition-PeanutButter-fact.png"
                            }
                        },
                        AmountInStock = 100
                    },
                    new Product
                    {
                        Category = ProductCategories.HealthyFood_Proteine_Chips,
                        Brand = ProductBrands.IronMaxx,
                        Name = "Protein Chips 40",
                        Cost = 100,
                        Description = "Protein Chips 40 от IronMaxx® - это самые вкусные чипсы со вкусом паприки с 20 г белка на пакет 50гр и богатые клетчаткой! \r\nПротеиновые чипсы изготавливаются из крахмалистой крупы Тапиоки (добывается из клубней растения Маниока) - не жарятся во фритюре, что делает их безопасными для здоровья. Вы можете без проблем добавлять их в качестве перекуса в свой рацион, дополняя его белком и клетчаткой. Protein Chips 40 практически без сахара и содержат на 70% меньше жира по сравнению с обычными жареными картофельными чипсами!",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/ProteinChips/IronMax_Chips.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/Iron-Maxx-Protein-Chips-fact.jpg"
                            }
                        },
                        AmountInStock = 100
                    },
                    new Product
                    {
                        Category = ProductCategories.HealthyFood_Granola,
                        Brand = ProductBrands.GO_ON,
                        Name = "Protein Granola",
                        Cost = 110,
                        Description = "Go OnProtein Granola - это гранола, содержащая растительный протеин, минералы и клетчатку. Это традиционный завтрак американцев, полезный и вкусный. Ее можно комбинировать с молоком и йогуртом, она легкая и сытная и очень просто готовится!\r\nВ состав Go OnProtein Granola входят минералы, необходимые для нормального функционирования организма, в частности магний, обеспечивающий сокращение мышц и устраняющий судороги, и калий, полезный для работы сердца.\r\nТакже гранола богата растительным протеином и клетчаткой. Протеин восстанавливает мышечные волокна и является материалом для строительства новых мышечных клеток. Клетчатка улучшает пищеварение и придает ощущение сытости.\r\nGo OnProtein Granola - это комбинация цельнозерновых хлопьев овса, орехов, сухофруктов и какао.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/Granola/GO_On_Granola.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/Go-ON-Granola-fact.png"
                            }
                        },
                        AmountInStock = 100
                    },
                    new Product
                    {
                        Category = ProductCategories.HealthyFood_Muesli,
                        Brand = ProductBrands.ALL_Nutrition,
                        Name = "NUTLOVE Protein Muesli",
                        Cost = 240,
                        Description = "NUTLOVE PROTEIN MUESLI это протеиновые мюсли с орехами, лиофилизированными бананами, тыквенными семечками, соевыми чипсами и шоколадным драже. Такое сочетание всех ингредиентов создает неповторимый вкус, обеспечивая ощущение сытости.\r\nПовышенное содержание белка, без добавления сахара, с добавлением фундука, кешью, соевых хлопьев и миндаля - благодаря этому NUTLOVE Protein Muesli имеет преимущество над традиционными мюслями.\r\nВысокое содержание клетчатки, которое Вы найдете в ALLNUTRITION PROTEIN MUESLI предотвращает чрезмерное переедание, что способствует похудению и позволяет поддерживать стройную фигуру. Рацион, богатый пищевыми волокнами, заставляет нас меньше есть, одновременно улучшая работу кишечника и, как следствие, мы теряем лишние килограммы.",
                        Pictures = new List < ProductPicture >()
                        {
                            new ProductPicture
                            {
                                Path="/prod_pictures/ProteinMuesli/NUTLOVE-Protein-Muesli.png" ,
                                NutritionPath = "/prod_pictures/Nutritions/NUTLOVE-PROTEIN-MUESLI-fact.png"
                            }
                        },
                        AmountInStock = 100
                    },
            };

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
                    new Flavor {  Name = "Печенье-крем" },
                    new Flavor {  Name = "Черника" },
                    new Flavor {  Name = "Фруктовый пунш" },
                    new Flavor {  Name = "Апельсин" },
                    new Flavor {  Name = "Колла" },
                    new Flavor {  Name = "Шоколад-банан" },
                    new Flavor {  Name = "Черный перец-соль" },
                    new Flavor {  Name = "Фундук-шоколад" }
            };

            dataBaseContext.Flavors.AddRange(FlavorsDb);

            dataBaseContext.Products.AddRange(SportProductsDb);
            dataBaseContext.Products.AddRange(HealthyFoodProductsDb);

            HealthyFoodProductsDb[0].Flavors.AddRange(new List<Flavor> { FlavorsDb[11]});
            HealthyFoodProductsDb[1].Flavors.AddRange(new List<Flavor> { FlavorsDb[11] });
            HealthyFoodProductsDb[2].Flavors.AddRange(new List<Flavor> { FlavorsDb[18] });
            HealthyFoodProductsDb[3].Flavors.AddRange(new List<Flavor> { FlavorsDb[19] });
            HealthyFoodProductsDb[4].Flavors.AddRange(new List<Flavor> { FlavorsDb[17] });

            SportProductsDb[0].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[1], FlavorsDb[2], FlavorsDb[3], FlavorsDb[4] });
            SportProductsDb[1].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[3], FlavorsDb[4], FlavorsDb[5], FlavorsDb[6] });
            SportProductsDb[2].Flavors.AddRange(new List<Flavor> { FlavorsDb[1], FlavorsDb[2], FlavorsDb[7], FlavorsDb[11] });
            SportProductsDb[3].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[4], FlavorsDb[7] });
            SportProductsDb[4].Flavors.AddRange(new List<Flavor> { FlavorsDb[8], FlavorsDb[9], FlavorsDb[10] });
            SportProductsDb[5].Flavors.AddRange(new List<Flavor> { FlavorsDb[7], FlavorsDb[10], FlavorsDb[11] });
            SportProductsDb[6].Flavors.AddRange(new List<Flavor> { FlavorsDb[7], FlavorsDb[8], FlavorsDb[9], FlavorsDb[10], FlavorsDb[11] });
            SportProductsDb[7].Flavors.AddRange(new List<Flavor> { FlavorsDb[1], FlavorsDb[2], FlavorsDb[4], FlavorsDb[5], FlavorsDb[6] });
            SportProductsDb[8].Flavors.AddRange(new List<Flavor> { FlavorsDb[11] });
            SportProductsDb[9].Flavors.AddRange(new List<Flavor> { FlavorsDb[11] });
            SportProductsDb[10].Flavors.AddRange(new List<Flavor> { FlavorsDb[12] });
            SportProductsDb[11].Flavors.AddRange(new List<Flavor> { FlavorsDb[8], FlavorsDb[9], FlavorsDb[11] });
            SportProductsDb[12].Flavors.AddRange(new List<Flavor> { FlavorsDb[0], FlavorsDb[1], FlavorsDb[2], FlavorsDb[3] });
            SportProductsDb[13].Flavors.AddRange(new List<Flavor> { FlavorsDb[9], FlavorsDb[13], FlavorsDb[14] });
            SportProductsDb[14].Flavors.AddRange(new List<Flavor> { FlavorsDb[5], FlavorsDb[6], FlavorsDb[7] });
            SportProductsDb[15].Flavors.AddRange(new List<Flavor> { FlavorsDb[11], FlavorsDb[15], FlavorsDb[8] });
            SportProductsDb[16].Flavors.AddRange(new List<Flavor> { FlavorsDb[13], FlavorsDb[14], FlavorsDb[16] });
            SportProductsDb[17].Flavors.AddRange(new List<Flavor> { FlavorsDb[15], FlavorsDb[14], FlavorsDb[9] });

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

            DiscountsDb[0].Products.AddRange(SportProductsDb);
            DiscountsDb[0].Products.AddRange(HealthyFoodProductsDb);

            DiscountsDb[0].Products.ForEach(x => x.DiscountCost = (decimal.Ceiling(x.Cost * 100) * (100 - DiscountsDb[0].DiscountPercent) / 100) / 100);


            await dataBaseContext.Discounts.ForEachAsync(x => x.Products.ForEach(q => q.DiscountCost = decimal.Ceiling(((q.Cost * 100) * (100 - x.DiscountPercent) / 100) / 100)));

            await dataBaseContext.SaveChangesAsync();
        }
    }
}
