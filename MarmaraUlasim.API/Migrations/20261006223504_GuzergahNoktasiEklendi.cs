using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class GuzergahNoktasiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuzergahNoktalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShapeId = table.Column<string>(type: "text", nullable: false),
                    Enlem = table.Column<double>(type: "double precision", nullable: false),
                    Boylam = table.Column<double>(type: "double precision", nullable: false),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    Mesafe = table.Column<double>(type: "double precision", nullable: true),
                    Kaynak = table.Column<string>(type: "text", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuzergahNoktalari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuzergahNoktalari_Kaynak_ShapeId_Sira",
                table: "GuzergahNoktalari",
                columns: new[] { "Kaynak", "ShapeId", "Sira" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuzergahNoktalari");
        }
    }
}
