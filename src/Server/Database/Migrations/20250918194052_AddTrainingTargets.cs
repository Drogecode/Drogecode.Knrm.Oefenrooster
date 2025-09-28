using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TrainingTargetSetId",
                table: "RoosterTraining",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainingTargetSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrainingTargetIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    ActiveSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReusableSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTargetSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingTargetSets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingTargetSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTargetSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingTargetSubjects_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingTargetSubjects_TrainingTargetSubjects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TrainingTargetSubjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    UrlDescription = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingTargets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingTargets_TrainingTargetSubjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "TrainingTargetSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingTargetUserResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingTargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterAvailableId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Result = table.Column<int>(type: "integer", nullable: false),
                    SetInBulk = table.Column<bool>(type: "boolean", nullable: false),
                    TrainingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SetBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTargetUserResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingTargetUserResult_RoosterAvailable_RoosterAvailableId",
                        column: x => x.RoosterAvailableId,
                        principalTable: "RoosterAvailable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingTargetUserResult_TrainingTargets_TrainingTargetId",
                        column: x => x.TrainingTargetId,
                        principalTable: "TrainingTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingTargetUserResult_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TrainingTargetSubjects",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "CustomerId", "DeletedBy", "DeletedOn", "Name", "Order", "ParentId" },
                values: new object[,]
                {
                    { new Guid("512af760-d93d-4f11-93fc-cdf0164ba0d7"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, "Communicatie", 20, null },
                    { new Guid("b0a94df1-f7cf-4408-86a4-cc4af0702f1b"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, "Algemene kennis", 10, null },
                    { new Guid("15b7f98c-8c47-47b3-9dd6-f9c92810aaa6"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, "Touwhandelingen", 10, new Guid("b0a94df1-f7cf-4408-86a4-cc4af0702f1b") },
                    { new Guid("590b6950-e75d-4ebf-9279-0d31e17ecd66"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, "Wal en water", 30, new Guid("512af760-d93d-4f11-93fc-cdf0164ba0d7") },
                    { new Guid("6cfb611e-63d7-4d33-be21-ebb8e8649cba"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, "Communicatie op het water", 40, new Guid("512af760-d93d-4f11-93fc-cdf0164ba0d7") }
                });

            migrationBuilder.InsertData(
                table: "TrainingTargets",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "CustomerId", "DeletedBy", "DeletedOn", "Description", "Group", "Name", "Order", "SubjectId", "Type", "Url", "UrlDescription" },
                values: new object[,]
                {
                    { new Guid("13c608b2-76c6-48da-918e-1052a7ae3e3a"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 0, "Uitvragen van de situatie", 40, new Guid("590b6950-e75d-4ebf-9279-0d31e17ecd66"), 0, "https://kompas.knrm.nl/Communicatie/Wal-en-water/Uitvragen-van-de-situatie", "Kompas" },
                    { new Guid("45035fb8-cc63-4d18-b1df-f6454a143eaf"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 0, "In- en uitmelden", 30, new Guid("590b6950-e75d-4ebf-9279-0d31e17ecd66"), 0, "https://kompas.knrm.nl/Communicatie/Wal-en-water/In-en-uitmelden", "Kompas" },
                    { new Guid("4dc2a888-8b95-4754-9d0d-e185789fe3a1"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 0, "SITREP", 60, new Guid("6cfb611e-63d7-4d33-be21-ebb8e8649cba"), 0, "https://kompas.knrm.nl/Communicatie/Communicatie-op-het-water/SITREP", "Kompas" },
                    { new Guid("55accb54-6a0a-449d-bd77-33aea502c355"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 0, "De paalsteek", 10, new Guid("15b7f98c-8c47-47b3-9dd6-f9c92810aaa6"), 0, "https://kompas.knrm.nl/Algemene-kennis/Touwhandelingen/Touwhandelingen-paalsteek", "Kompas" },
                    { new Guid("5d0e590e-b955-43be-bd46-0edf84472a2b"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 0, "Een paalsteek leggen", 20, new Guid("15b7f98c-8c47-47b3-9dd6-f9c92810aaa6"), 1, "https://kompas.knrm.nl/Algemene-kennis/Touwhandelingen/Touwhandelingen-paalsteek-leggen", "Kompas" },
                    { new Guid("622ce20e-5ee4-4caa-9b3b-78aae55ac2b5"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 0, "Werken met DSC", 50, new Guid("6cfb611e-63d7-4d33-be21-ebb8e8649cba"), 0, "https://kompas.knrm.nl/Communicatie/Communicatie-op-het-water/Werken-met-DSC", "Kompas" },
                    { new Guid("f8e01dba-2d80-47d5-a254-96e20e929bea"), new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"), new DateTime(2025, 8, 14, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, null, 2, "Uitvragen van de situatie", 50, new Guid("590b6950-e75d-4ebf-9279-0d31e17ecd66"), 1, "https://kompas.knrm.nl/Communicatie/Wal-en-water/Communicatie-uitvragen-van-de-situatie", "Kompas" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_TrainingTargetSetId",
                table: "RoosterTraining",
                column: "TrainingTargetSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargets_CustomerId",
                table: "TrainingTargets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargets_SubjectId",
                table: "TrainingTargets",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargetSets_CustomerId",
                table: "TrainingTargetSets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargetSubjects_CustomerId",
                table: "TrainingTargetSubjects",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargetSubjects_ParentId",
                table: "TrainingTargetSubjects",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargetUserResult_RoosterAvailableId",
                table: "TrainingTargetUserResult",
                column: "RoosterAvailableId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargetUserResult_TrainingTargetId",
                table: "TrainingTargetUserResult",
                column: "TrainingTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTargetUserResult_UserId",
                table: "TrainingTargetUserResult",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterTraining_TrainingTargetSets_TrainingTargetSetId",
                table: "RoosterTraining",
                column: "TrainingTargetSetId",
                principalTable: "TrainingTargetSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoosterTraining_TrainingTargetSets_TrainingTargetSetId",
                table: "RoosterTraining");

            migrationBuilder.DropTable(
                name: "TrainingTargetSets");

            migrationBuilder.DropTable(
                name: "TrainingTargetUserResult");

            migrationBuilder.DropTable(
                name: "TrainingTargets");

            migrationBuilder.DropTable(
                name: "TrainingTargetSubjects");

            migrationBuilder.DropIndex(
                name: "IX_RoosterTraining_TrainingTargetSetId",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "TrainingTargetSetId",
                table: "RoosterTraining");
        }
    }
}
