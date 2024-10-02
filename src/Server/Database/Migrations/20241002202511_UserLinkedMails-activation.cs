using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserLinkedMailsactivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivateKey",
                table: "UserLinkedMails",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivateRequestedOn",
                table: "UserLinkedMails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivationFailedAttempts",
                table: "UserLinkedMails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserLinkedMails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "UserLinkedMails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivateKey",
                table: "UserLinkedMails");

            migrationBuilder.DropColumn(
                name: "ActivateRequestedOn",
                table: "UserLinkedMails");

            migrationBuilder.DropColumn(
                name: "ActivationFailedAttempts",
                table: "UserLinkedMails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserLinkedMails");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "UserLinkedMails");
        }
    }
}
