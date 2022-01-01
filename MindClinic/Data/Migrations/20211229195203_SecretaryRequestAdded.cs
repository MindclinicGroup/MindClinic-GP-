using Microsoft.EntityFrameworkCore.Migrations;

namespace MindClinic.Data.Migrations
{
    public partial class SecretaryRequestAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecretaryRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecretaryID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretaryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecretaryRequests_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SecretaryRequests_AspNetUsers_SecretaryID",
                        column: x => x.SecretaryID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecretaryRequests_DoctorId",
                table: "SecretaryRequests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_SecretaryRequests_SecretaryID",
                table: "SecretaryRequests",
                column: "SecretaryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecretaryRequests");
        }
    }
}
