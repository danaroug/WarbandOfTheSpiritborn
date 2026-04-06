using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class RenameEventPropTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutText",
                table: "About",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutText",
                table: "About");
        }
    }
}
