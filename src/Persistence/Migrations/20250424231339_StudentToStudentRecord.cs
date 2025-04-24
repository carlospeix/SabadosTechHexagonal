using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StudentToStudentRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaregivingRelationships_Students_StudentId",
                table: "CaregivingRelationships");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Grades",
                newName: "StudentName");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "CaregivingRelationships",
                newName: "StudentRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_CaregivingRelationships_StudentId",
                table: "CaregivingRelationships",
                newName: "IX_CaregivingRelationships_StudentRecordId");

            migrationBuilder.CreateTable(
                name: "StudentRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentGradeId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRecords_Grades_CurrentGradeId",
                        column: x => x.CurrentGradeId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRecords_CurrentGradeId",
                table: "StudentRecords",
                column: "CurrentGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRecords_TenantId",
                table: "StudentRecords",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaregivingRelationships_StudentRecords_StudentRecordId",
                table: "CaregivingRelationships",
                column: "StudentRecordId",
                principalTable: "StudentRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaregivingRelationships_StudentRecords_StudentRecordId",
                table: "CaregivingRelationships");

            migrationBuilder.DropTable(
                name: "StudentRecords");

            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "Grades",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "StudentRecordId",
                table: "CaregivingRelationships",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_CaregivingRelationships_StudentRecordId",
                table: "CaregivingRelationships",
                newName: "IX_CaregivingRelationships_StudentId");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_GradeId",
                table: "Students",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId",
                table: "Students",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaregivingRelationships_Students_StudentId",
                table: "CaregivingRelationships",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
