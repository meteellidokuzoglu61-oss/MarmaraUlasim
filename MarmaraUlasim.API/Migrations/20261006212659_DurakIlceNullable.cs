using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmaraUlasim.API.Migrations
{
    /// <inheritdoc />
    public partial class DurakIlceNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Duraklar_Ilceler_IlceId",
                table: "Duraklar");

            migrationBuilder.AlterColumn<int>(
                name: "IlceId",
                table: "Duraklar",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Duraklar_Ilceler_IlceId",
                table: "Duraklar",
                column: "IlceId",
                principalTable: "Ilceler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Duraklar_Ilceler_IlceId",
                table: "Duraklar");

            migrationBuilder.AlterColumn<int>(
                name: "IlceId",
                table: "Duraklar",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Duraklar_Ilceler_IlceId",
                table: "Duraklar",
                column: "IlceId",
                principalTable: "Ilceler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
