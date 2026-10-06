using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeferDuraklar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Seferler_SeferKodu",
                table: "Seferler",
                column: "SeferKodu");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Duraklar_DurakKodu",
                table: "Duraklar",
                column: "DurakKodu");

            migrationBuilder.CreateTable(
                name: "SeferDuraklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeferKodu = table.Column<string>(type: "text", nullable: false),
                    DurakKodu = table.Column<string>(type: "text", nullable: false),
                    DurakSirasi = table.Column<int>(type: "integer", nullable: false),
                    VarisSaati = table.Column<TimeSpan>(type: "interval", nullable: true),
                    KalkisSaati = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeferDuraklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeferDuraklar_Duraklar_DurakKodu",
                        column: x => x.DurakKodu,
                        principalTable: "Duraklar",
                        principalColumn: "DurakKodu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeferDuraklar_Seferler_SeferKodu",
                        column: x => x.SeferKodu,
                        principalTable: "Seferler",
                        principalColumn: "SeferKodu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeferDuraklar_DurakKodu",
                table: "SeferDuraklar",
                column: "DurakKodu");

            migrationBuilder.CreateIndex(
                name: "IX_SeferDuraklar_SeferKodu_DurakSirasi",
                table: "SeferDuraklar",
                columns: new[] { "SeferKodu", "DurakSirasi" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeferDuraklar");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Seferler_SeferKodu",
                table: "Seferler");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Duraklar_DurakKodu",
                table: "Duraklar");
        }
    }
}
