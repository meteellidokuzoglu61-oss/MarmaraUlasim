using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeferler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Hatlar_HatKodu",
                table: "Hatlar",
                column: "HatKodu");

            migrationBuilder.CreateTable(
                name: "Seferler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeferKodu = table.Column<string>(type: "text", nullable: false),
                    HatKodu = table.Column<string>(type: "text", nullable: false),
                    ServisKodu = table.Column<string>(type: "text", nullable: true),
                    VarisYonu = table.Column<string>(type: "text", nullable: true),
                    Kaynak = table.Column<string>(type: "text", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seferler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seferler_Hatlar_HatKodu",
                        column: x => x.HatKodu,
                        principalTable: "Hatlar",
                        principalColumn: "HatKodu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seferler_HatKodu",
                table: "Seferler",
                column: "HatKodu");

            migrationBuilder.CreateIndex(
                name: "IX_Seferler_Kaynak_SeferKodu",
                table: "Seferler",
                columns: new[] { "Kaynak", "SeferKodu" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seferler");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Hatlar_HatKodu",
                table: "Hatlar");
        }
    }
}
