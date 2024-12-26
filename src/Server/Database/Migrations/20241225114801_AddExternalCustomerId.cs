using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "UserRoles",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            /*migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                column: "ExternalId",
                value: "");

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("68be785c-1226-4280-a110-bd87f328951f"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                column: "CreatedBy",
                value: new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                column: "ExternalId",
                value: "2197a054-e81f-4720-9f08-321377398cb6");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "ExternalId",
                value: "287359b1-2035-435b-97b0-eb260dc497d6");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"),
                column: "ExternalId",
                value: "2956c6f9-6b83-46eb-8890-dbb640fd5023");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                column: "ExternalId",
                value: "54aace50-0e1f-4c35-a1b3-87c9ff6bd743");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("90a40128-183f-408b-aa64-eb3b279a7042"),
                column: "ExternalId",
                value: "90a40128-183f-408b-aa64-eb3b279a7042");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                column: "ExternalId",
                value: "afb45395-89ee-413d-9385-21962772dbda");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d526e5ed-e838-499d-a96c-62180db28bed"),
                column: "ExternalId",
                value: "d526e5ed-e838-499d-a96c-62180db28bed");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "ExternalId",
                value: "d72ed2e9-911e-4ee5-b07e-cbd5917d432b");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"),
                column: "ExternalId",
                value: "f06a00e3-62c9-4ba5-baea-84a5ba10f53a");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                column: "ExternalId",
                value: "f5b0bab6-6fdf-457d-855d-bbea6ea57bd5");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "ExternalId",
                value: "f6b0c571-9050-40d6-bf58-807981e5ed6e");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Customers");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "UserRoles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            /*migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("68be785c-1226-4280-a110-bd87f328951f"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                column: "CreatedBy",
                value: new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                column: "ExternalId",
                value: new Guid("2197a054-e81f-4720-9f08-321377398cb6"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "ExternalId",
                value: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"),
                column: "ExternalId",
                value: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                column: "ExternalId",
                value: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("90a40128-183f-408b-aa64-eb3b279a7042"),
                column: "ExternalId",
                value: new Guid("90a40128-183f-408b-aa64-eb3b279a7042"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                column: "ExternalId",
                value: new Guid("afb45395-89ee-413d-9385-21962772dbda"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d526e5ed-e838-499d-a96c-62180db28bed"),
                column: "ExternalId",
                value: new Guid("d526e5ed-e838-499d-a96c-62180db28bed"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "ExternalId",
                value: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"),
                column: "ExternalId",
                value: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                column: "ExternalId",
                value: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "ExternalId",
                value: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"));*/
        }
    }
}
