using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomerceCon_Apis.Migrations
{
    /// <inheritdoc />
    public partial class agregarmensaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MensajeRecibidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensaje = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeRecibidos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensajeRecibidos_Mensaje",
                table: "MensajeRecibidos",
                column: "Mensaje",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajeRecibidos");
        }
    }
}
