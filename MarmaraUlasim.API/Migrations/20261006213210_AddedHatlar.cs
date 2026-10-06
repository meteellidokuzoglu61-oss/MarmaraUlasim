using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedHatlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hatlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HatKodu = table.Column<string>(type: "text", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Tip = table.Column<string>(type: "text", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    Kaynak = table.Column<string>(type: "text", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hatlar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hatlar");
        }
    }
}
