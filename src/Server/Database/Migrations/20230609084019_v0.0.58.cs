using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0058 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RoosterTrainingType",
                columns: new[] { "Id", "ColorDark", "ColorLight", "CountToTrainingTarget", "CustomerId", "IsDefault", "Name", "Order" },
                values: new object[] { new Guid("68be785c-1226-4280-a110-bd87f328951f"), null, "#000000", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "Proeve van Bekwaamheid", 80 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("68be785c-1226-4280-a110-bd87f328951f"));
        }
    }
}
