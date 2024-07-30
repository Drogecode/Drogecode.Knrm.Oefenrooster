using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReportActionTrainingTotalTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalFullHours",
                table: "ReportTrainings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalMinutes",
                table: "ReportTrainings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFullHours",
                table: "ReportActions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalMinutes",
                table: "ReportActions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalFullHours",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "TotalMinutes",
                table: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "TotalFullHours",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "TotalMinutes",
                table: "ReportActions");
        }
    }
}
