using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class dblinkforlinktrainingvehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "VehicleIds",
                value: null);*/

            migrationBuilder.CreateIndex(
                name: "IX_LinkVehicleTraining_RoosterTrainingId",
                table: "LinkVehicleTraining",
                column: "RoosterTrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkVehicleTraining_VehicleId",
                table: "LinkVehicleTraining",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkVehicleTraining_RoosterTraining_RoosterTrainingId",
                table: "LinkVehicleTraining",
                column: "RoosterTrainingId",
                principalTable: "RoosterTraining",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinkVehicleTraining_Vehicle_VehicleId",
                table: "LinkVehicleTraining",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkVehicleTraining_RoosterTraining_RoosterTrainingId",
                table: "LinkVehicleTraining");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkVehicleTraining_Vehicle_VehicleId",
                table: "LinkVehicleTraining");

            migrationBuilder.DropIndex(
                name: "IX_LinkVehicleTraining_RoosterTrainingId",
                table: "LinkVehicleTraining");

            migrationBuilder.DropIndex(
                name: "IX_LinkVehicleTraining_VehicleId",
                table: "LinkVehicleTraining");

            /*migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "VehicleIds",
                value: new List<Guid> { new Guid("4589535c-9064-4448-bc01-3b5a00e9410d") });*/
        }
    }
}
