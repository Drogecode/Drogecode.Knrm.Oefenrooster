using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class PreventDeletingHuiActionsAndTrainings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SetByExternalOn",
                table: "ReportUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SetByExternalOn",
                table: "ReportTrainings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SetByExternalOn",
                table: "ReportActions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SetByExternalOn",
                table: "LinkReportTrainingRoosterTraining",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetByExternalOn",
                table: "ReportUsers");

            migrationBuilder.DropColumn(
                name: "SetByExternalOn",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "SetByExternalOn",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "SetByExternalOn",
                table: "LinkReportTrainingRoosterTraining");
        }
    }
}
