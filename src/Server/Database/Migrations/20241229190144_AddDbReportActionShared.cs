using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDbReportActionShared : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActions_Customers_CustomerId",
                table: "ReportActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTrainings_Customers_CustomerId",
                table: "ReportTrainings");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportUsers_ReportActions_DbReportActionId",
                table: "ReportUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportUsers_ReportTrainings_DbReportTrainingId",
                table: "ReportUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportUsers",
                table: "ReportUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTrainings",
                table: "ReportTrainings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportActions",
                table: "ReportActions");

            migrationBuilder.RenameTable(
                name: "ReportUsers",
                newName: "ReportUser");

            migrationBuilder.RenameTable(
                name: "ReportTrainings",
                newName: "ReportTraining");

            migrationBuilder.RenameTable(
                name: "ReportActions",
                newName: "ReportAction");

            migrationBuilder.RenameIndex(
                name: "IX_ReportUsers_DbReportTrainingId",
                table: "ReportUser",
                newName: "IX_ReportUser_DbReportTrainingId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportUsers_DbReportActionId",
                table: "ReportUser",
                newName: "IX_ReportUser_DbReportActionId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportTrainings_CustomerId",
                table: "ReportTraining",
                newName: "IX_ReportTraining_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportActions_CustomerId",
                table: "ReportAction",
                newName: "IX_ReportAction_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportUser",
                table: "ReportUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTraining",
                table: "ReportTraining",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportAction",
                table: "ReportAction",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ReportActionShared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SelectedUsers = table.Column<List<Guid>>(type: "uuid[]", nullable: true),
                    Types = table.Column<List<string>>(type: "text[]", nullable: true),
                    Search = table.Column<List<string>>(type: "text[]", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HashedPassword = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportActionShared", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportActionShared_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportActionShared_CustomerId",
                table: "ReportActionShared",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportAction_Customers_CustomerId",
                table: "ReportAction",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTraining_Customers_CustomerId",
                table: "ReportTraining",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportUser_ReportAction_DbReportActionId",
                table: "ReportUser",
                column: "DbReportActionId",
                principalTable: "ReportAction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportUser_ReportTraining_DbReportTrainingId",
                table: "ReportUser",
                column: "DbReportTrainingId",
                principalTable: "ReportTraining",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportAction_Customers_CustomerId",
                table: "ReportAction");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTraining_Customers_CustomerId",
                table: "ReportTraining");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportUser_ReportAction_DbReportActionId",
                table: "ReportUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportUser_ReportTraining_DbReportTrainingId",
                table: "ReportUser");

            migrationBuilder.DropTable(
                name: "ReportActionShared");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportUser",
                table: "ReportUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTraining",
                table: "ReportTraining");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportAction",
                table: "ReportAction");

            migrationBuilder.RenameTable(
                name: "ReportUser",
                newName: "ReportUsers");

            migrationBuilder.RenameTable(
                name: "ReportTraining",
                newName: "ReportTrainings");

            migrationBuilder.RenameTable(
                name: "ReportAction",
                newName: "ReportActions");

            migrationBuilder.RenameIndex(
                name: "IX_ReportUser_DbReportTrainingId",
                table: "ReportUsers",
                newName: "IX_ReportUsers_DbReportTrainingId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportUser_DbReportActionId",
                table: "ReportUsers",
                newName: "IX_ReportUsers_DbReportActionId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportTraining_CustomerId",
                table: "ReportTrainings",
                newName: "IX_ReportTrainings_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportAction_CustomerId",
                table: "ReportActions",
                newName: "IX_ReportActions_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportUsers",
                table: "ReportUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTrainings",
                table: "ReportTrainings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportActions",
                table: "ReportActions",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ReportUsers_ReportActions_DbReportActionId",
                table: "ReportUsers",
                column: "DbReportActionId",
                principalTable: "ReportActions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportUsers_ReportTrainings_DbReportTrainingId",
                table: "ReportUsers",
                column: "DbReportTrainingId",
                principalTable: "ReportTrainings",
                principalColumn: "Id");
        }
    }
}
