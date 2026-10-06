using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class MahallelerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mahalleler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApiId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    IlceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mahalleler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mahalleler_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mahalleler_ApiId",
                table: "Mahalleler",
                column: "ApiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mahalleler_IlceId",
                table: "Mahalleler",
                column: "IlceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mahalleler");
        }
    }
}
