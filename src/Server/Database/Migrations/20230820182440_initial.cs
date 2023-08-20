using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<double>(type: "double precision", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    Prio = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTrainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTrainings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinkVehicleTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterTrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vehicle = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkVehicleTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkVehicleTraining_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreComAlert",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Alert = table.Column<string>(type: "text", nullable: true),
                    Raw = table.Column<string>(type: "text", nullable: true),
                    SendTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreComAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreComAlert_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterItemDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DateStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFullDay = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterItemDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterItemDay_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterItemMonth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<short>(type: "smallint", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterItemMonth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterItemMonth_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterTrainingType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ColorLight = table.Column<string>(type: "text", nullable: true),
                    ColorDark = table.Column<string>(type: "text", nullable: true),
                    TextColorLight = table.Column<string>(type: "text", nullable: true),
                    TextColorDark = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CountToTrainingTarget = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterTrainingType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterTrainingType_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFunctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    TrainingTarget = table.Column<int>(type: "integer", nullable: false),
                    TrainingOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFunctions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Accesses = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Deletedby = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SharePointID = table.Column<string>(type: "text", nullable: true),
                    DrogeCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    DbReportActionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DbReportTrainingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportUsers_ReportActions_DbReportActionId",
                        column: x => x.DbReportActionId,
                        principalTable: "ReportActions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportUsers_ReportTrainings_DbReportTrainingId",
                        column: x => x.DbReportTrainingId,
                        principalTable: "ReportTrainings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoosterDefault",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterTrainingTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    WeekDay = table.Column<int>(type: "integer", nullable: false),
                    TimeStart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    CountToTrainingTarget = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterDefault", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterDefault_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterDefault_RoosterTrainingType_RoosterTrainingTypeId",
                        column: x => x.RoosterTrainingTypeId,
                        principalTable: "RoosterTrainingType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFunctionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserFunctions_UserFunctionId",
                        column: x => x.UserFunctionId,
                        principalTable: "UserFunctions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoosterTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterDefaultId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoosterTrainingTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DateStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CountToTrainingTarget = table.Column<bool>(type: "boolean", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterTraining_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterTraining_RoosterDefault_RoosterDefaultId",
                        column: x => x.RoosterDefaultId,
                        principalTable: "RoosterDefault",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoosterTraining_RoosterTrainingType_RoosterTrainingTypeId",
                        column: x => x.RoosterTrainingTypeId,
                        principalTable: "RoosterTrainingType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditType = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ObjectKey = table.Column<Guid>(type: "uuid", nullable: true),
                    ObjectName = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Audit_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDefaultAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterDefaultId = table.Column<Guid>(type: "uuid", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Assigned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefaultAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDefaultAvailable_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDefaultAvailable_RoosterDefault_RoosterDefaultId",
                        column: x => x.RoosterDefaultId,
                        principalTable: "RoosterDefault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDefaultAvailable_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHolidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHolidays_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHolidays_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFunctionId = table.Column<Guid>(type: "uuid", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CalendarEventId = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: true),
                    SetBy = table.Column<int>(type: "integer", nullable: false),
                    Assigned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_RoosterTraining_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "RoosterTraining",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_UserFunctions_UserFunctionId",
                        column: x => x.UserFunctionId,
                        principalTable: "UserFunctions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Created", "Name" },
                values: new object[] { new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new DateTime(2022, 10, 12, 18, 12, 5, 0, DateTimeKind.Utc), "KNRM Huizen" });

            migrationBuilder.InsertData(
                table: "RoosterItemMonth",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "CustomerId", "Month", "Order", "Severity", "Text", "Type", "Year" },
                values: new object[,]
                {
                    { new Guid("01a17983-9bbe-4bfc-b152-f73c1869393d"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)4, 0, 0, "KNRM Kompas onderwerp; Veiligheid", 1, null },
                    { new Guid("36e33dc3-8bb8-4096-a127-c3ee04a0e694"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)3, 0, 0, "KNRM Kompas onderwerp; SAR & Hulpverlening", 1, null },
                    { new Guid("4a009dd3-db02-4668-bbb0-9a9298c23d58"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)6, 0, 0, "KNRM Kompas onderwerp; EHBO & Procedures", 1, null },
                    { new Guid("5208deef-4529-4a30-a00e-22737cf52183"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)2, 0, 0, "KNRM Kompas onderwerp;  Algemene kennis & Communicatie", 1, null },
                    { new Guid("7696579c-403c-4a98-b30e-b19f1e90ffd0"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)7, 1, 1, "Geen ingeroosterde oefeningen in verband met het hoogseizoen", 1, null },
                    { new Guid("857e2ee9-8f2f-407e-9ec8-a0eaa853b957"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)1, 0, 0, "KNRM Kompas onderwerp; GEEN", 1, null },
                    { new Guid("d7d80ee0-0e73-426f-84b2-2040057c2f7a"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)1, 1, 1, "Geen ingeroosterde oefeningen in verband met het winterseizoen", 1, null },
                    { new Guid("e4c00e6b-14d5-4609-bff3-6a6533557a0b"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)7, 0, 0, "KNRM Kompas onderwerp; Techiek & Varen", 1, null },
                    { new Guid("f9d140f0-58fa-4c9a-a845-0eb5bad2814f"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)5, 0, 0, "KNRM Kompas onderwerp; Navigatie", 1, null }
                });

            migrationBuilder.InsertData(
                table: "RoosterTrainingType",
                columns: new[] { "Id", "ColorDark", "ColorLight", "CountToTrainingTarget", "CreatedBy", "CreatedDate", "CustomerId", "IsActive", "IsDefault", "Name", "Order", "TextColorDark", "TextColorLight", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"), "rgb(244,47,70)", "rgb(242,28,13)", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Brandweer", 40, "#C0C0C0", "#FFFFFF", null, null },
                    { new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"), "", "rgb(25,169,140)", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "een op een", 30, null, null, null, null },
                    { new Guid("6153a297-9486-43de-91e8-22d107da2b21"), "#3BB9FF", "#ADD8E6", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Evenement", 60, null, null, null, null },
                    { new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"), "#5f6138", "#919454", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Techniek", 70, null, null, null, null },
                    { new Guid("68be785c-1226-4280-a110-bd87f328951f"), null, "#000000", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Proeve van Bekwaamheid", 80, "#C0C0C0", "#FFFFFF", null, null },
                    { new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), "#ffffff4c", "#bdbdbdff", true, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, true, "Kompas oefening", 10, null, null, null, null },
                    { new Guid("80108015-87a7-4453-a1af-d81d15fe3582"), "rgb(214,143,0)", "rgb(214,129,0)", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "EHBO", 20, null, null, null, null },
                    { new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"), "rgb(13,222,156)", "rgb(0,235,98)", false, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 26, 12, 12, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "HRB oefening", 50, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "UserFunctions",
                columns: new[] { "Id", "CustomerId", "IsActive", "IsDefault", "Name", "Order", "TrainingOnly", "TrainingTarget" },
                values: new object[,]
                {
                    { new Guid("0a0a2c2d-15c7-4205-93a2-621de3c30db1"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Extra", 300, false, 0 },
                    { new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, true, "Opstapper op proef", 80, false, 0 },
                    { new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Opstapper", 60, false, 2 },
                    { new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Schipper", 20, false, 2 },
                    { new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "HRB Aankomend opstapper", 100, false, 0 },
                    { new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Waarnemer", 180, true, 0 },
                    { new Guid("a23f1f39-275e-4e21-901f-4878b73d3ede"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, false, "Inactief", 400, false, 0 },
                    { new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Schipper I.O.", 30, false, 2 },
                    { new Guid("d23de705-d950-4833-8b94-aa531022d450"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Kompas leider", 10, true, 0 },
                    { new Guid("feb3641f-9941-4db7-a202-14263d706516"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, false, "Aankomend opstapper", 70, false, 2 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Accesses", "CustomerId", "Name" },
                values: new object[,]
                {
                    { new Guid("287359b1-2035-435b-97b0-eb260dc497d6"), "configure_training-types", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "Admin" },
                    { new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"), "users_counter,users_details", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "Users" },
                    { new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"), "scheduler", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "Scheduler" }
                });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "Id", "Code", "CustomerId", "DeletedOn", "Deletedby", "IsActive", "IsDefault", "Name", "Order" },
                values: new object[,]
                {
                    { new Guid("4589535c-9064-4448-bc01-3b5a00e9410d"), "NWI", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, true, true, "Nikolaas Wijsenbeek", 10 },
                    { new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"), "HZN018", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, true, false, "Vlet", 30 },
                    { new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"), "HZR", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, true, false, "De Huizer", 20 },
                    { new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"), "Wal", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, true, false, "Wal", 100 }
                });

            migrationBuilder.InsertData(
                table: "RoosterDefault",
                columns: new[] { "Id", "CountToTrainingTarget", "CustomerId", "RoosterTrainingTypeId", "TimeEnd", "TimeStart", "TimeZone", "ValidFrom", "ValidUntil", "WeekDay" },
                values: new object[,]
                {
                    { new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(13, 0, 0), new TimeOnly(10, 0, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 0 },
                    { new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(13, 0, 0), new TimeOnly(10, 0, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 6 },
                    { new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(16, 0, 0), new TimeOnly(13, 0, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 6 },
                    { new Guid("4142048e-82dc-4015-aab7-1b519da01238"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 1 },
                    { new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 2 },
                    { new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(16, 0, 0), new TimeOnly(13, 0, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 0 },
                    { new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 4 },
                    { new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"), true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), "Europe/Amsterdam", new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_CustomerId",
                table: "Audit",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_UserId",
                table: "Audit",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkVehicleTraining_CustomerId",
                table: "LinkVehicleTraining",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PreComAlert_CustomerId",
                table: "PreComAlert",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportUsers_DbReportActionId",
                table: "ReportUsers",
                column: "DbReportActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportUsers_DbReportTrainingId",
                table: "ReportUsers",
                column: "DbReportTrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_CustomerId",
                table: "RoosterAvailable",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_TrainingId",
                table: "RoosterAvailable",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_UserFunctionId",
                table: "RoosterAvailable",
                column: "UserFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_UserId",
                table: "RoosterAvailable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_VehicleId",
                table: "RoosterAvailable",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterDefault_CustomerId",
                table: "RoosterDefault",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterDefault_RoosterTrainingTypeId",
                table: "RoosterDefault",
                column: "RoosterTrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterItemDay_CustomerId",
                table: "RoosterItemDay",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterItemMonth_CustomerId",
                table: "RoosterItemMonth",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_CustomerId",
                table: "RoosterTraining",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_RoosterDefaultId",
                table: "RoosterTraining",
                column: "RoosterDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_RoosterTrainingTypeId",
                table: "RoosterTraining",
                column: "RoosterTrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTrainingType_CustomerId",
                table: "RoosterTrainingType",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_CustomerId",
                table: "UserDefaultAvailable",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_RoosterDefaultId",
                table: "UserDefaultAvailable",
                column: "RoosterDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_UserId",
                table: "UserDefaultAvailable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFunctions_CustomerId",
                table: "UserFunctions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHolidays_CustomerId",
                table: "UserHolidays",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHolidays_UserId",
                table: "UserHolidays",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_CustomerId",
                table: "UserRoles",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CustomerId",
                table: "Users",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserFunctionId",
                table: "Users",
                column: "UserFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CustomerId",
                table: "Vehicle",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "LinkVehicleTraining");

            migrationBuilder.DropTable(
                name: "PreComAlert");

            migrationBuilder.DropTable(
                name: "ReportUsers");

            migrationBuilder.DropTable(
                name: "RoosterAvailable");

            migrationBuilder.DropTable(
                name: "RoosterItemDay");

            migrationBuilder.DropTable(
                name: "RoosterItemMonth");

            migrationBuilder.DropTable(
                name: "UserDefaultAvailable");

            migrationBuilder.DropTable(
                name: "UserHolidays");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "ReportActions");

            migrationBuilder.DropTable(
                name: "ReportTrainings");

            migrationBuilder.DropTable(
                name: "RoosterTraining");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RoosterDefault");

            migrationBuilder.DropTable(
                name: "UserFunctions");

            migrationBuilder.DropTable(
                name: "RoosterTrainingType");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
