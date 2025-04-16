using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CaregivingRelationshipsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CaregivingRelationships",
                table: "CaregivingRelationships");

            migrationBuilder.DropIndex(
                name: "IX_CaregivingRelationships_ParentId",
                table: "CaregivingRelationships");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CaregivingRelationships");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaregivingRelationships",
                table: "CaregivingRelationships",
                columns: new[] { "ParentId", "StudentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CaregivingRelationships",
                table: "CaregivingRelationships");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CaregivingRelationships",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaregivingRelationships",
                table: "CaregivingRelationships",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CaregivingRelationships_ParentId",
                table: "CaregivingRelationships",
                column: "ParentId");
        }
    }
}
