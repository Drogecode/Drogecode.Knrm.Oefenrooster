using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class PreComAddIp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "PreComAlert",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

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
                name: "Ip",
                table: "PreComAlert");

            /*migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "VehicleIds",
                value: new List<Guid> { new Guid("4589535c-9064-4448-bc01-3b5a00e9410d") });*/
        }
    }
}
