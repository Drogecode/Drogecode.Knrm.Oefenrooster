using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class FullActionsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Number",
                table: "ReportActions",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Boat",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallMadeBy",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Causes",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Commencement",
                table: "ReportActions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Completedby",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CountAnimals",
                table: "ReportActions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CountSailors",
                table: "ReportActions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CountSaved",
                table: "ReportActions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ReportActions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "ReportActions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ForTheBenefitOf",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FunctioningMaterial",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GolfHight",
                table: "ReportActions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Implications",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemsWithWeed",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Request",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sight",
                table: "ReportActions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WaterTemperature",
                table: "ReportActions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherCondition",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WindDirection",
                table: "ReportActions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WindPower",
                table: "ReportActions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Boat",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "CallMadeBy",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Causes",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Commencement",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Completedby",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "CountAnimals",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "CountSailors",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "CountSaved",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "End",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "ForTheBenefitOf",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "FunctioningMaterial",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "GolfHight",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Implications",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "ProblemsWithWeed",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Request",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Sight",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "WaterTemperature",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "WeatherCondition",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "WindDirection",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "WindPower",
                table: "ReportActions");

            migrationBuilder.AlterColumn<double>(
                name: "Number",
                table: "ReportActions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
