using Microsoft.EntityFrameworkCore.Migrations;

namespace MindClinic.Data.Migrations
{
    public partial class addedDoctorInformations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricePerSession = table.Column<int>(type: "int", nullable: false),
                    userID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.id);
                    table.ForeignKey(
                        name: "FK_Doctors_AspNetUsers_userID",
                        column: x => x.userID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    award = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.id);
                    table.ForeignKey(
                        name: "FK_Awards_Doctors_doctorId",
                        column: x => x.doctorId,
                        principalTable: "Doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    yearOfCompletion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Educations_Doctors_doctorId",
                        column: x => x.doctorId,
                        principalTable: "Doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Awards_doctorId",
                table: "Awards",
                column: "doctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_userID",
                table: "Doctors",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_doctorId",
                table: "Educations",
                column: "doctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "Doctors");
        }
    }
}
