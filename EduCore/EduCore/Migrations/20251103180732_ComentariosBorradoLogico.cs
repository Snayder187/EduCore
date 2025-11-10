using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Migrations
{
    /// <inheritdoc />
    public partial class ComentariosBorradoLogico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstadoBorrado",
                table: "Comentarios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoBorrado",
                table: "Comentarios");
        }
    }
}
