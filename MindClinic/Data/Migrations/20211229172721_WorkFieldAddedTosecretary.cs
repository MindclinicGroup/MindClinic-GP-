using Microsoft.EntityFrameworkCore.Migrations;

namespace MindClinic.Data.Migrations
{
    public partial class WorkFieldAddedTosecretary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Work",
                table: "Secretary",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Work",
                table: "Secretary");
        }
    }
}
