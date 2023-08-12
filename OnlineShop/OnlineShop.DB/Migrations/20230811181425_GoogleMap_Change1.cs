using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.DB.Migrations
{
    /// <inheritdoc />
    public partial class GoogleMap_Change1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "GeoLong",
                table: "ShopContacts",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "GeoLat",
                table: "ShopContacts",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.443195462540587, 30.623799652983227 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.417734500100003, 30.632090487314496 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.504420827695476, 30.49735526833221 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.4240376432695, 30.457578810654578 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.397052417251487, 30.505197383670733 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.488240667136559, 30.506997268331233 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.44915822528278, 30.59671118367401 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.402293318405434, 30.625064454834703 });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { 50.456834616417034, 30.365450183674429 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GeoLong",
                table: "ShopContacts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "GeoLat",
                table: "ShopContacts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.44319546254059", "30.623799652983227" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.4177345001", "30.632090487314496" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.504420827695476", "30.49735526833221" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.4240376432695", "30.457578810654578" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.39705241725149", "30.505197383670733" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.48824066713656", "30.506997268331233" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.44915822528278", "30.59671118367401" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.402293318405434", "30.625064454834703" });

            migrationBuilder.UpdateData(
                table: "ShopContacts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "GeoLat", "GeoLong" },
                values: new object[] { "50.456834616417034", "30.36545018367443" });
        }
    }
}
