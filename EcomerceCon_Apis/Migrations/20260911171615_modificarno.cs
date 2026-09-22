using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    /// <inheritdoc />
    public partial class modificarno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertaProducto_Ofertas_ofertasId",
                table: "OfertaProducto");

            migrationBuilder.DropForeignKey(
                name: "FK_OfertaProducto_Productos_ProductosId",
                table: "OfertaProducto");

            migrationBuilder.RenameColumn(
                name: "ofertasId",
                table: "OfertaProducto",
                newName: "ProductoId");

            migrationBuilder.RenameColumn(
                name: "ProductosId",
                table: "OfertaProducto",
                newName: "OfertaId");

            migrationBuilder.RenameIndex(
                name: "IX_OfertaProducto_ofertasId",
                table: "OfertaProducto",
                newName: "IX_OfertaProducto_ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertaProducto_Ofertas_OfertaId",
                table: "OfertaProducto",
                column: "OfertaId",
                principalTable: "Ofertas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfertaProducto_Productos_ProductoId",
                table: "OfertaProducto",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertaProducto_Ofertas_OfertaId",
                table: "OfertaProducto");

            migrationBuilder.DropForeignKey(
                name: "FK_OfertaProducto_Productos_ProductoId",
                table: "OfertaProducto");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "OfertaProducto",
                newName: "ofertasId");

            migrationBuilder.RenameColumn(
                name: "OfertaId",
                table: "OfertaProducto",
                newName: "ProductosId");

            migrationBuilder.RenameIndex(
                name: "IX_OfertaProducto_ProductoId",
                table: "OfertaProducto",
                newName: "IX_OfertaProducto_ofertasId");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertaProducto_Ofertas_ofertasId",
                table: "OfertaProducto",
                column: "ofertasId",
                principalTable: "Ofertas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfertaProducto_Productos_ProductosId",
                table: "OfertaProducto",
                column: "ProductosId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
