using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDuraklar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Duraklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DurakKodu = table.Column<string>(type: "text", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Enlem = table.Column<double>(type: "double precision", nullable: false),
                    Boylam = table.Column<double>(type: "double precision", nullable: false),
                    IlceId = table.Column<int>(type: "integer", nullable: false),
                    MahalleId = table.Column<int>(type: "integer", nullable: true),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duraklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duraklar_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Duraklar_Mahalleler_MahalleId",
                        column: x => x.MahalleId,
                        principalTable: "Mahalleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Duraklar_DurakKodu",
                table: "Duraklar",
                column: "DurakKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Duraklar_IlceId",
                table: "Duraklar",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Duraklar_MahalleId",
                table: "Duraklar",
                column: "MahalleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Duraklar");
        }
    }
}
