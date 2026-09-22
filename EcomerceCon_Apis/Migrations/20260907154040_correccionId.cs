using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    /// <inheritdoc />
    public partial class correccionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carritos_AspNetUsers_usuarioId",
                table: "Carritos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCarritos_Carritos_carritoId",
                table: "DetalleCarritos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCarritos_Productos_productoId",
                table: "DetalleCarritos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallePedidos_Productos_productoId",
                table: "DetallePedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "clave",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "correo",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "productoId",
                table: "DetallePedidos",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallePedidos_productoId",
                table: "DetallePedidos",
                newName: "IX_DetallePedidos_ProductoId");

            migrationBuilder.RenameColumn(
                name: "productoId",
                table: "DetalleCarritos",
                newName: "ProductoId");

            migrationBuilder.RenameColumn(
                name: "carritoId",
                table: "DetalleCarritos",
                newName: "CarritoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleCarritos_productoId",
                table: "DetalleCarritos",
                newName: "IX_DetalleCarritos_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleCarritos_carritoId",
                table: "DetalleCarritos",
                newName: "IX_DetalleCarritos_CarritoId");

            migrationBuilder.RenameColumn(
                name: "usuarioId",
                table: "Carritos",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Carritos_usuarioId",
                table: "Carritos",
                newName: "IX_Carritos_UsuarioId");

            migrationBuilder.AddColumn<float>(
                name: "precio",
                table: "Productos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Carritos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Carritos_AspNetUsers_UsuarioId",
                table: "Carritos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCarritos_Carritos_CarritoId",
                table: "DetalleCarritos",
                column: "CarritoId",
                principalTable: "Carritos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCarritos_Productos_ProductoId",
                table: "DetalleCarritos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallePedidos_Productos_ProductoId",
                table: "DetallePedidos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carritos_AspNetUsers_UsuarioId",
                table: "Carritos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCarritos_Carritos_CarritoId",
                table: "DetalleCarritos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCarritos_Productos_ProductoId",
                table: "DetalleCarritos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallePedidos_Productos_ProductoId",
                table: "DetallePedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "precio",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "DetallePedidos",
                newName: "productoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallePedidos_ProductoId",
                table: "DetallePedidos",
                newName: "IX_DetallePedidos_productoId");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "DetalleCarritos",
                newName: "productoId");

            migrationBuilder.RenameColumn(
                name: "CarritoId",
                table: "DetalleCarritos",
                newName: "carritoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleCarritos_ProductoId",
                table: "DetalleCarritos",
                newName: "IX_DetalleCarritos_productoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleCarritos_CarritoId",
                table: "DetalleCarritos",
                newName: "IX_DetalleCarritos_carritoId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Carritos",
                newName: "usuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Carritos_UsuarioId",
                table: "Carritos",
                newName: "IX_Carritos_usuarioId");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "usuarioId",
                table: "Carritos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "clave",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "correo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Carritos_AspNetUsers_usuarioId",
                table: "Carritos",
                column: "usuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCarritos_Carritos_carritoId",
                table: "DetalleCarritos",
                column: "carritoId",
                principalTable: "Carritos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCarritos_Productos_productoId",
                table: "DetalleCarritos",
                column: "productoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallePedidos_Productos_productoId",
                table: "DetallePedidos",
                column: "productoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
