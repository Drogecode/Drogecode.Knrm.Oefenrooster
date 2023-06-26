using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0087 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Vehicle",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Default",
                table: "UserFunctions",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "UserFunctions",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Pin",
                table: "RoosterTraining",
                newName: "IsPinned");

            migrationBuilder.AddColumn<Guid>(
                name: "Deletedby",
                table: "Vehicle",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "RoosterTrainingType",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RoosterTrainingType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "RoosterTrainingType",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "RoosterTrainingType",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("68be785c-1226-4280-a110-bd87f328951f"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                columns: new[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), null, null });

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("4589535c-9064-4448-bc01-3b5a00e9410d"),
                column: "Deletedby",
                value: null);

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"),
                column: "Deletedby",
                value: null);

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"),
                column: "Deletedby",
                value: null);

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"),
                column: "Deletedby",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deletedby",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoosterTrainingType");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RoosterTrainingType");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RoosterTrainingType");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "RoosterTrainingType");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Vehicle",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "UserFunctions",
                newName: "Default");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "UserFunctions",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "IsPinned",
                table: "RoosterTraining",
                newName: "Pin");
        }
    }
}
