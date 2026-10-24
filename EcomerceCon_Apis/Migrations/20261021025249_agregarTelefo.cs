using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    /// <inheritdoc />
    public partial class agregarTelefo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Numero_telefono",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numero_telefono",
                table: "AspNetUsers");
        }
    }
}
