using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReportActionDeparture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Departure",
                table: "ReportActions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            /*migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "VehicleIds",
                value: new List<Guid> { new Guid("4589535c-9064-4448-bc01-3b5a00e9410d") });*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departure",
                table: "ReportActions");

            /*migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "VehicleIds",
                value: new List<Guid> { new Guid("4589535c-9064-4448-bc01-3b5a00e9410d") });*/
        }
    }
}
