using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    /// <inheritdoc />
    public partial class CambiartipoDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "MontoAdescontar",
                table: "Ofertas",
                type: "real",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

           

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MontoAdescontar",
                table: "Ofertas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
         
        }
    }
}
