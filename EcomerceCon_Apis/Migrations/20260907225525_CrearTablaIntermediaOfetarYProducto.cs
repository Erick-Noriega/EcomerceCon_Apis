using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaIntermediaOfetarYProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ofertas_Productos_ProductoId",
                table: "Ofertas");

            migrationBuilder.DropIndex(
                name: "IX_Ofertas_ProductoId",
                table: "Ofertas");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Ofertas");

            migrationBuilder.AlterColumn<int>(
                name: "cantidad",
                table: "Productos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "OfertaProducto",
                columns: table => new
                {
                    ProductosId = table.Column<int>(type: "int", nullable: false),
                    ofertasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfertaProducto", x => new { x.ProductosId, x.ofertasId });
                    table.ForeignKey(
                        name: "FK_OfertaProducto_Ofertas_ofertasId",
                        column: x => x.ofertasId,
                        principalTable: "Ofertas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfertaProducto_Productos_ProductosId",
                        column: x => x.ProductosId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<float>(type: "real", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransaccionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfertaProducto_ofertasId",
                table: "OfertaProducto",
                column: "ofertasId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PedidoId",
                table: "Pagos",
                column: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfertaProducto");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.AlterColumn<string>(
                name: "cantidad",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "Ofertas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_ProductoId",
                table: "Ofertas",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ofertas_Productos_ProductoId",
                table: "Ofertas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }
    }
}
