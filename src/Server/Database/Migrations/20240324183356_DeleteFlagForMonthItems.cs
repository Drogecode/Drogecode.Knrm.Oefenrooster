using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class DeleteFlagForMonthItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "RoosterItemMonth",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "RoosterItemMonth",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("01a17983-9bbe-4bfc-b152-f73c1869393d"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("36e33dc3-8bb8-4096-a127-c3ee04a0e694"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("40cbe7bc-4ed4-4897-8cb9-357785cb58c9"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("4100bf4d-368e-47d1-b652-8fec191b4934"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("48045049-e9cb-4d86-8f34-85578f015f76"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("4a009dd3-db02-4668-bbb0-9a9298c23d58"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("5208deef-4529-4a30-a00e-22737cf52183"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("532b68c3-328f-45a2-8a7e-fdf7f9eee111"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("7696579c-403c-4a98-b30e-b19f1e90ffd0"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("857e2ee9-8f2f-407e-9ec8-a0eaa853b957"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("b858e7fc-02ea-4a2a-a49d-f55c3a912c9c"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("ca0fe95d-3b84-4136-bcee-ce080228c324"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("d7d80ee0-0e73-426f-84b2-2040057c2f7a"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("e4c00e6b-14d5-4609-bff3-6a6533557a0b"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("f9d140f0-58fa-4c9a-a845-0eb5bad2814f"),
                columns: new[] { "DeletedBy", "DeletedOn" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RoosterItemMonth");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "RoosterItemMonth");
        }
    }
}
