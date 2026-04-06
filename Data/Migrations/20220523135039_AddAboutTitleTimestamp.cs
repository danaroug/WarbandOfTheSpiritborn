using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class AddAboutTitleTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutTitle",
                table: "About",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutTitle",
                table: "About");
        }
    }
}
