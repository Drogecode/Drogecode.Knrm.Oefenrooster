using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class SettingTypAsEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Setting",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Setting",
                table: "CustomerSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "UserSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Name",
                table: "UserSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Name",
                table: "CustomerSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CustomerSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "UserSettings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Setting",
                table: "UserSettings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Setting",
                table: "CustomerSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
