using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class vehicleadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Vehicle",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Createdby",
                table: "Vehicle",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "RoosterItemMonth",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "CustomerId", "Month", "Order", "Severity", "Text", "Type", "Year" },
                values: new object[,]
                {
                    { new Guid("40cbe7bc-4ed4-4897-8cb9-357785cb58c9"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)10, 0, 0, "KNRM Kompas onderwerp; ntb", 1, null },
                    { new Guid("4100bf4d-368e-47d1-b652-8fec191b4934"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)8, 1, 1, "Geen ingeroosterde oefeningen in verband met het hoogseizoen", 1, null },
                    { new Guid("48045049-e9cb-4d86-8f34-85578f015f76"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)8, 0, 0, "KNRM Kompas onderwerp; ntb", 1, null },
                    { new Guid("532b68c3-328f-45a2-8a7e-fdf7f9eee111"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)9, 0, 0, "KNRM Kompas onderwerp; ntb", 1, null },
                    { new Guid("b858e7fc-02ea-4a2a-a49d-f55c3a912c9c"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)12, 0, 0, "KNRM Kompas onderwerp; ntb", 1, null },
                    { new Guid("ca0fe95d-3b84-4136-bcee-ce080228c324"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)11, 0, 0, "KNRM Kompas onderwerp; ntb", 1, null }
                });

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("4589535c-9064-4448-bc01-3b5a00e9410d"),
                columns: new[] { "CreatedOn", "Createdby" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"),
                columns: new[] { "CreatedOn", "Createdby" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"),
                columns: new[] { "CreatedOn", "Createdby" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"),
                columns: new[] { "CreatedOn", "Createdby" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("40cbe7bc-4ed4-4897-8cb9-357785cb58c9"));

            migrationBuilder.DeleteData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("4100bf4d-368e-47d1-b652-8fec191b4934"));

            migrationBuilder.DeleteData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("48045049-e9cb-4d86-8f34-85578f015f76"));

            migrationBuilder.DeleteData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("532b68c3-328f-45a2-8a7e-fdf7f9eee111"));

            migrationBuilder.DeleteData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("b858e7fc-02ea-4a2a-a49d-f55c3a912c9c"));

            migrationBuilder.DeleteData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("ca0fe95d-3b84-4136-bcee-ce080228c324"));

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Createdby",
                table: "Vehicle");
        }
    }
}
