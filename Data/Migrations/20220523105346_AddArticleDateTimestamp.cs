using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class AddArticleDateTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArticleDate",
                table: "Blog",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleDate",
                table: "Blog");
        }
    }
}
