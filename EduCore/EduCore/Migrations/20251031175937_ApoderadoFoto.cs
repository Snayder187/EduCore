using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Migrations
{
    /// <inheritdoc />
    public partial class ApoderadoFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Apoderados",
                type: "varchar(max)",
                unicode: false,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Apoderados");
        }
    }
}
