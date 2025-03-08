using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mapping.Migrations
{
    /// <inheritdoc />
    public partial class AdjustingIdCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Configurations",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Configurations",
                newName: "id");
        }
    }
}
