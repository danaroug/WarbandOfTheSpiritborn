using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WarbandOfTheSpiritborn.Data.Migrations
{
    public partial class BuildPropsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuildAuthor",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BuildDate",
                table: "Builds",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BuildName",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainSkills",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherItems",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rotation",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondarySkills",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stat",
                table: "Builds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeaponSet",
                table: "Builds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildAuthor",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "BuildDate",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "BuildName",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "Item",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "MainSkills",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "OtherItems",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "Rotation",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "SecondarySkills",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "WeaponSet",
                table: "Builds");
        }
    }
}
