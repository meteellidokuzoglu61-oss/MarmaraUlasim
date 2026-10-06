using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDurakKaynak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Duraklar_DurakKodu",
                table: "Duraklar");

            migrationBuilder.AddColumn<string>(
                name: "Kaynak",
                table: "Duraklar",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Duraklar_Kaynak_DurakKodu",
                table: "Duraklar",
                columns: new[] { "Kaynak", "DurakKodu" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Duraklar_Kaynak_DurakKodu",
                table: "Duraklar");

            migrationBuilder.DropColumn(
                name: "Kaynak",
                table: "Duraklar");

            migrationBuilder.CreateIndex(
                name: "IX_Duraklar_DurakKodu",
                table: "Duraklar",
                column: "DurakKodu",
                unique: true);
        }
    }
}
