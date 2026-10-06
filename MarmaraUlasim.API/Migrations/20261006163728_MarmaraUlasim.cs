using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class MarmaraUlasim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Iller",
                columns: new[] { "Id", "Ad", "PlakaKodu" },
                values: new object[,]
                {
                    { 1, "İstanbul", 34 },
                    { 2, "Edirne", 22 },
                    { 3, "Kırklareli", 39 },
                    { 4, "Tekirdağ", 59 },
                    { 5, "Çanakkale", 17 },
                    { 6, "Balıkesir", 10 },
                    { 7, "Bursa", 16 },
                    { 8, "Yalova", 77 },
                    { 9, "Kocaeli", 41 },
                    { 10, "Sakarya", 54 },
                    { 11, "Bilecik", 11 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Iller",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}
