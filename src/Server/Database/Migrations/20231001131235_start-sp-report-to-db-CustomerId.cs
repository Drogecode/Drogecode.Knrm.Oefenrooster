using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class startspreporttodbCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "ReportTrainings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "ReportActions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ReportTrainings_CustomerId",
                table: "ReportTrainings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportActions_CustomerId",
                table: "ReportActions",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActions_Customers_CustomerId",
                table: "ReportActions",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTrainings_Customers_CustomerId",
                table: "ReportTrainings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActions_Customers_CustomerId",
                table: "ReportActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTrainings_Customers_CustomerId",
                table: "ReportTrainings");

            migrationBuilder.DropIndex(
                name: "IX_ReportTrainings_CustomerId",
                table: "ReportTrainings");

            migrationBuilder.DropIndex(
                name: "IX_ReportActions_CustomerId",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ReportActions");
        }
    }
}
