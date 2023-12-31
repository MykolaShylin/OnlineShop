﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineShop.DB.Contexts;

#nullable disable

namespace OnlineShop.DB.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    [Migration("20230812112419_ProductBrandEdit")]
    partial class ProductBrandEdit
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FlavorProduct", b =>
                {
                    b.Property<int>("FlavorsId")
                        .HasColumnType("int");

                    b.Property<int>("ProductsId")
                        .HasColumnType("int");

                    b.HasKey("FlavorsId", "ProductsId");

                    b.HasIndex("ProductsId");

                    b.ToTable("FlavorProduct");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Basket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Basket");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.BasketItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<Guid>("BasketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ProductInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BasketId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ProductInfoId");

                    b.ToTable("BasketItems");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.ChoosingProductInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("DiscountPercent")
                        .HasColumnType("int");

                    b.Property<int>("FlavorId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ChoosingProductInfo");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.ComparingProducts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FlavorId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FlavorId");

                    b.HasIndex("ProductId");

                    b.ToTable("ComparingProducts");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.DeliveryInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliveryType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DeliveryInfo");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Discount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DiscountPercent")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Flavor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Flavors");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.GoogleMapShopInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("GeoLat")
                        .HasColumnType("float");

                    b.Property<double>("GeoLong")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkingHours")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ShopContacts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Adress = "пр. Мира 2/3",
                            GeoLat = 50.443195462540587,
                            GeoLong = 30.623799652983227,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(097) 526-96-88",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 2,
                            Adress = "ул. Драгоманова, 2-Б",
                            GeoLat = 50.417734500100003,
                            GeoLong = 30.632090487314496,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(096) 579-14-83",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 3,
                            Adress = "пр. Оболонский, 7",
                            GeoLat = 50.504420827695476,
                            GeoLong = 30.49735526833221,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(096) 532-41-95",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 4,
                            Adress = "пр. Воздухофлотский, 52",
                            GeoLat = 50.4240376432695,
                            GeoLong = 30.457578810654578,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(068) 113-22-25",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 5,
                            Adress = "ул. Васильковская, 6",
                            GeoLat = 50.397052417251487,
                            GeoLong = 30.505197383670733,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(096) 657-44-73",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 6,
                            Adress = "пр. Степана Бандеры, 20-Б",
                            GeoLat = 50.488240667136559,
                            GeoLong = 30.506997268331233,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(068) 536-97-06",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 7,
                            Adress = "ул. Раисы Окипной, 3",
                            GeoLat = 50.44915822528278,
                            GeoLong = 30.59671118367401,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(073) 887-03-30",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 8,
                            Adress = "ул. Княжий Затон, 9",
                            GeoLat = 50.402293318405434,
                            GeoLong = 30.625064454834703,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(098) 200-37-37",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        },
                        new
                        {
                            Id = 9,
                            Adress = "пр. Победы, 136",
                            GeoLat = 50.456834616417034,
                            GeoLong = 30.365450183674429,
                            Name = "Магазин спортивного питания Bull Body",
                            Phone = "(073) 108-12-11",
                            WorkingHours = "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00"
                        });
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("OrderDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("deliveryInfoId")
                        .HasColumnType("int");

                    b.Property<int>("orderStatus")
                        .IsConcurrencyToken()
                        .HasColumnType("int");

                    b.Property<int>("payInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("deliveryInfoId");

                    b.HasIndex("payInfoId");

                    b.ToTable("ClosedOrders");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.PayInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PayType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PayInfo");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AmountInStock")
                        .HasColumnType("int");

                    b.Property<int>("Brand")
                        .HasColumnType("int");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<byte[]>("Concurrency")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("Concurrency");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DiscountCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("DiscountDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DiscountId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DiscountId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.ProductPicture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("FlavorProduct", b =>
                {
                    b.HasOne("OnlineShop.DB.Models.Flavor", null)
                        .WithMany()
                        .HasForeignKey("FlavorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineShop.DB.Models.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlineShop.DB.Models.BasketItem", b =>
                {
                    b.HasOne("OnlineShop.DB.Models.Basket", "Basket")
                        .WithMany("Items")
                        .HasForeignKey("BasketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineShop.DB.Models.Order", null)
                        .WithMany("Items")
                        .HasForeignKey("OrderId");

                    b.HasOne("OnlineShop.DB.Models.Product", "Product")
                        .WithMany("BasketItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineShop.DB.Models.ChoosingProductInfo", "ProductInfo")
                        .WithMany("BasketItems")
                        .HasForeignKey("ProductInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Basket");

                    b.Navigation("Product");

                    b.Navigation("ProductInfo");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.ComparingProducts", b =>
                {
                    b.HasOne("OnlineShop.DB.Models.Flavor", "Flavor")
                        .WithMany()
                        .HasForeignKey("FlavorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineShop.DB.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Flavor");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Order", b =>
                {
                    b.HasOne("OnlineShop.DB.Models.DeliveryInfo", "deliveryInfo")
                        .WithMany()
                        .HasForeignKey("deliveryInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineShop.DB.Models.PayInfo", "payInfo")
                        .WithMany()
                        .HasForeignKey("payInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("deliveryInfo");

                    b.Navigation("payInfo");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Product", b =>
                {
                    b.HasOne("OnlineShop.DB.Models.Discount", null)
                        .WithMany("Products")
                        .HasForeignKey("DiscountId");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.ProductPicture", b =>
                {
                    b.HasOne("OnlineShop.DB.Models.Product", null)
                        .WithMany("Pictures")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Basket", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.ChoosingProductInfo", b =>
                {
                    b.Navigation("BasketItems");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Discount", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("OnlineShop.DB.Models.Product", b =>
                {
                    b.Navigation("BasketItems");

                    b.Navigation("Pictures");
                });
#pragma warning restore 612, 618
        }
    }
}
