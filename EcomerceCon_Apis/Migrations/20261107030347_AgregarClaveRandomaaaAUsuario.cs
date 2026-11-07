using EcomerceCon_Apis.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    [DbContext(typeof(ProductosDbcontext))]
    [Migration("20261107030347_AgregarClaveRandomaaaAUsuario")]
    public partial class AgregarClaveRandomaaaAUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaveRandomaaa",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaveRandomaaa",
                table: "AspNetUsers");
        }
    }
}
