using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoreReportActionTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Boat",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Commencement",
                table: "ReportTrainings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ReportTrainings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "ReportTrainings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FunctioningMaterial",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GolfHight",
                table: "ReportTrainings",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "ReportTrainings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OdataEtag",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemsWithWeed",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sight",
                table: "ReportTrainings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeTraining",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WaterTemperature",
                table: "ReportTrainings",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherCondition",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WindDirection",
                table: "ReportTrainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WindPower",
                table: "ReportTrainings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdataEtag",
                table: "ReportActions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "Boat",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "Commencement",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "End",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "FunctioningMaterial",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "GolfHight",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "OdataEtag",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "ProblemsWithWeed",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "Sight",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "TypeTraining",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "WaterTemperature",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "WeatherCondition",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "WindDirection",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "WindPower",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "OdataEtag",
                table: "ReportActions");
        }
    }
}
