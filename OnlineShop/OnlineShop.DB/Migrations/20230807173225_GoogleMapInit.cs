using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineShop.DB.Migrations
{
    /// <inheritdoc />
    public partial class GoogleMapInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiscountDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ShopContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeoLat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeoLong = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopContacts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ShopContacts",
                columns: new[] { "Id", "Adress", "GeoLat", "GeoLong", "Name", "Phone", "WorkingHours" },
                values: new object[,]
                {
                    { 1, "пр. Мира 2/3", "50.44319546254059", "30.623799652983227", "Магазин спортивного питания Bull Body", "(097) 526-96-88", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 2, "ул. Драгоманова, 2-Б", "50.4177345001", "30.632090487314496", "Магазин спортивного питания Bull Body", "(096) 579-14-83", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 3, "пр. Оболонский, 7", "50.504420827695476", "30.49735526833221", "Магазин спортивного питания Bull Body", "(096) 532-41-95", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 4, "пр. Воздухофлотский, 52", "50.4240376432695", "30.457578810654578", "Магазин спортивного питания Bull Body", "(068) 113-22-25", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 5, "ул. Васильковская, 6", "50.39705241725149", "30.505197383670733", "Магазин спортивного питания Bull Body", "(096) 657-44-73", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 6, "пр. Степана Бандеры, 20-Б", "50.48824066713656", "30.506997268331233", "Магазин спортивного питания Bull Body", "(068) 536-97-06", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 7, "ул. Раисы Окипной, 3", "50.44915822528278", "30.59671118367401", "Магазин спортивного питания Bull Body", "(073) 887-03-30", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 8, "ул. Княжий Затон, 9", "50.402293318405434", "30.625064454834703", "Магазин спортивного питания Bull Body", "(098) 200-37-37", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" },
                    { 9, "пр. Победы, 136", "50.456834616417034", "30.36545018367443", "Магазин спортивного питания Bull Body", "(073) 108-12-11", "Пн-Пт 10:00−20:00\r\nСб-Вс 10:00−19:00" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopContacts");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
