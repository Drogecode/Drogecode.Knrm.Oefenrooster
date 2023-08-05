using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0089 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "RoosterItemMonth",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "RoosterItemMonth",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "RoosterItemDay",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "RoosterItemDay",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ReportActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<double>(type: "double precision", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    Prio = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTrainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTrainings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SharePointID = table.Column<string>(type: "text", nullable: true),
                    DrogeCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    DbReportActionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DbReportTrainingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportUsers_ReportActions_DbReportActionId",
                        column: x => x.DbReportActionId,
                        principalTable: "ReportActions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportUsers_ReportTrainings_DbReportTrainingId",
                        column: x => x.DbReportTrainingId,
                        principalTable: "ReportTrainings",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("01a17983-9bbe-4bfc-b152-f73c1869393d"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("36e33dc3-8bb8-4096-a127-c3ee04a0e694"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("4a009dd3-db02-4668-bbb0-9a9298c23d58"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("5208deef-4529-4a30-a00e-22737cf52183"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("7696579c-403c-4a98-b30e-b19f1e90ffd0"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("857e2ee9-8f2f-407e-9ec8-a0eaa853b957"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("d7d80ee0-0e73-426f-84b2-2040057c2f7a"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("e4c00e6b-14d5-4609-bff3-6a6533557a0b"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("f9d140f0-58fa-4c9a-a845-0eb5bad2814f"),
                columns: new[] { "CreatedBy", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_ReportUsers_DbReportActionId",
                table: "ReportUsers",
                column: "DbReportActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportUsers_DbReportTrainingId",
                table: "ReportUsers",
                column: "DbReportTrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportUsers");

            migrationBuilder.DropTable(
                name: "ReportActions");

            migrationBuilder.DropTable(
                name: "ReportTrainings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoosterItemMonth");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "RoosterItemMonth");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoosterItemDay");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "RoosterItemDay");
        }
    }
}
