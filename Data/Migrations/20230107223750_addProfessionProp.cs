using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class addProfessionProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "Builds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profession",
                table: "Builds");
        }
    }
}
