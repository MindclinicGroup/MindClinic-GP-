using Microsoft.EntityFrameworkCore.Migrations;

namespace MindClinic.Data.Migrations
{
    public partial class DurationRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Appointments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
