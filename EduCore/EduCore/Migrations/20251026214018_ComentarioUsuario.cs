using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Migrations
{
    /// <inheritdoc />
    public partial class ComentarioUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "FechaModifica",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "IpAddressModifica",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "IpAddressRegistro",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioModifica",
                table: "Comentarios");

            migrationBuilder.RenameColumn(
                name: "UsuarioRegistro",
                table: "Comentarios",
                newName: "ApoderadoId");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Comentarios",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ApoderadoId",
                table: "Comentarios",
                column: "ApoderadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Apoderados_ApoderadoId",
                table: "Comentarios",
                column: "ApoderadoId",
                principalTable: "Apoderados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Apoderados_ApoderadoId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_ApoderadoId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Comentarios");

            migrationBuilder.RenameColumn(
                name: "ApoderadoId",
                table: "Comentarios",
                newName: "UsuarioRegistro");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Comentarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModifica",
                table: "Comentarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Comentarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IpAddressModifica",
                table: "Comentarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddressRegistro",
                table: "Comentarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModifica",
                table: "Comentarios",
                type: "int",
                nullable: true);
        }
    }
}
