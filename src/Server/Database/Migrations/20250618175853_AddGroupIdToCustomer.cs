﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIdToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "Customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                column: "GroupId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Customers");
        }
    }
}
