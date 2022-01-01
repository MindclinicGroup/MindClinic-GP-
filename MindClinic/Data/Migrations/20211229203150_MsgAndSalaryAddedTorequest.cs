using Microsoft.EntityFrameworkCore.Migrations;

namespace MindClinic.Data.Migrations
{
    public partial class MsgAndSalaryAddedTorequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "SecretaryRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Salary",
                table: "SecretaryRequests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "SecretaryRequests");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "SecretaryRequests");
        }
    }
}
