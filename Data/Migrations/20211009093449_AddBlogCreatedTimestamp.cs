using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class AddBlogCreatedTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Blogpost",
                table: "Blog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Blog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reply",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blogpost",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Reply",
                table: "Blog");
        }
    }
}
