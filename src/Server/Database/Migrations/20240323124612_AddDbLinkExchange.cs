using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDbLinkExchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeId",
                table: "Vehicle",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LinkExchangeId",
                table: "RoosterAvailable",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LinkExchange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CalendarEventId = table.Column<string>(type: "text", nullable: true),
                    IsSet = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkExchange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkExchange_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                column: "Accesses",
                value: "full_training_history,full_action_history,scheduler_other");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "Accesses",
                value: "configure_training-types,users_settings,scheduler_dayitem,scheduler_monthitem,scheduler_history");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "Name",
                value: "Users admin");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table,scheduler_past,scheduler_dayitem,scheduler_other,scheduler_monthitem");

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("4589535c-9064-4448-bc01-3b5a00e9410d"),
                column: "ExchangeId",
                value: new Guid("dbaeaa44-d318-464e-ac39-f85029dd9e8f"));

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"),
                column: "ExchangeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"),
                column: "ExchangeId",
                value: new Guid("731fa301-d7cc-41de-9063-f86a32c2b25b"));

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("d2b920a5-e8ec-4d47-a280-8f88eae914c1"),
                column: "ExchangeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"),
                column: "ExchangeId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_LinkExchangeId",
                table: "RoosterAvailable",
                column: "LinkExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkExchange_CustomerId",
                table: "LinkExchange",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterAvailable_LinkExchange_LinkExchangeId",
                table: "RoosterAvailable",
                column: "LinkExchangeId",
                principalTable: "LinkExchange",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoosterAvailable_LinkExchange_LinkExchangeId",
                table: "RoosterAvailable");

            migrationBuilder.DropTable(
                name: "LinkExchange");

            migrationBuilder.DropIndex(
                name: "IX_RoosterAvailable_LinkExchangeId",
                table: "RoosterAvailable");

            migrationBuilder.DropColumn(
                name: "ExchangeId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "LinkExchangeId",
                table: "RoosterAvailable");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                column: "Accesses",
                value: "full_training_history,full_action_history");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "Accesses",
                value: "configure_training-types,users_settings");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "Name",
                value: "Users");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table,scheduler_past,scheduler_dayitem,scheduler_other");
        }
    }
}
