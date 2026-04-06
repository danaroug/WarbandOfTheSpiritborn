using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class AddPropTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Blogpost",
                table: "Blog",
                newName: "BlogPost");

            migrationBuilder.AddColumn<string>(
                name: "BlogAuthor",
                table: "Blog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlogName",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogAuthor",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "BlogName",
                table: "Blog");

            migrationBuilder.RenameColumn(
                name: "BlogPost",
                table: "Blog",
                newName: "Blogpost");
        }
    }
}
