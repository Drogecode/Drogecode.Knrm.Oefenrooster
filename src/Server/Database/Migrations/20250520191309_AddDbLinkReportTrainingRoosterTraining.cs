using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDbLinkReportTrainingRoosterTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkReportTrainingRoosterTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterTrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportTrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkReportTrainingRoosterTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkReportTrainingRoosterTraining_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkReportTrainingRoosterTraining_ReportTrainings_ReportTra~",
                        column: x => x.ReportTrainingId,
                        principalTable: "ReportTrainings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkReportTrainingRoosterTraining_RoosterTraining_RoosterTr~",
                        column: x => x.RoosterTrainingId,
                        principalTable: "RoosterTraining",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkReportTrainingRoosterTraining_CustomerId",
                table: "LinkReportTrainingRoosterTraining",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkReportTrainingRoosterTraining_ReportTrainingId",
                table: "LinkReportTrainingRoosterTraining",
                column: "ReportTrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkReportTrainingRoosterTraining_RoosterTrainingId",
                table: "LinkReportTrainingRoosterTraining",
                column: "RoosterTrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkReportTrainingRoosterTraining");
        }
    }
}
