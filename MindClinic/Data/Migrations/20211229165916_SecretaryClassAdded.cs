using Microsoft.EntityFrameworkCore.Migrations;

namespace MindClinic.Data.Migrations
{
    public partial class SecretaryClassAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Secretary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecretaryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Salary = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secretary_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Secretary_AspNetUsers_SecretaryId",
                        column: x => x.SecretaryId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Secretary_DoctorId",
                table: "Secretary",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Secretary_SecretaryId",
                table: "Secretary",
                column: "SecretaryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Secretary");
        }
    }
}
