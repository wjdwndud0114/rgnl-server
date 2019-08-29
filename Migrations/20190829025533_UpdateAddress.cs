using Microsoft.EntityFrameworkCore.Migrations;

namespace rgnl_server.Migrations
{
    public partial class UpdateAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Profile");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "Profile",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "Profile");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Profile",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
