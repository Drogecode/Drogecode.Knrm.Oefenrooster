﻿// <auto-generated />
using System;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230128163117_v0.0.9")]
    partial class v009
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Database.Models.DbUsers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbAudit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AuditType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CustomerId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<Guid?>("ObjectKey")
                        .HasColumnType("uuid");

                    b.Property<string>("ObjectName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("UserId");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Customers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            Created = new DateTime(2022, 10, 12, 18, 12, 5, 0, DateTimeKind.Utc),
                            Name = "KNRM Huizen"
                        });
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterAvailable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Assigned")
                        .HasColumnType("boolean");

                    b.Property<int?>("Available")
                        .HasColumnType("integer");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<Guid>("TrainingId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("TrainingId");

                    b.HasIndex("UserId");

                    b.ToTable("RoosterAvailable");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterDefault", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<int>("WeekDay")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("RoosterDefault");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(21, 30, 0),
                            StartTime = new TimeOnly(19, 30, 0),
                            WeekDay = 1
                        },
                        new
                        {
                            Id = new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(21, 30, 0),
                            StartTime = new TimeOnly(19, 30, 0),
                            WeekDay = 2
                        },
                        new
                        {
                            Id = new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(21, 30, 0),
                            StartTime = new TimeOnly(19, 30, 0),
                            WeekDay = 3
                        },
                        new
                        {
                            Id = new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(21, 30, 0),
                            StartTime = new TimeOnly(19, 30, 0),
                            WeekDay = 4
                        },
                        new
                        {
                            Id = new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(13, 0, 0),
                            StartTime = new TimeOnly(10, 0, 0),
                            WeekDay = 6
                        },
                        new
                        {
                            Id = new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(16, 0, 0),
                            StartTime = new TimeOnly(13, 0, 0),
                            WeekDay = 6
                        },
                        new
                        {
                            Id = new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(13, 0, 0),
                            StartTime = new TimeOnly(10, 0, 0),
                            WeekDay = 0
                        },
                        new
                        {
                            Id = new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                            CustomerId = new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                            EndTime = new TimeOnly(16, 0, 0),
                            StartTime = new TimeOnly(13, 0, 0),
                            WeekDay = 0
                        });
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterTraining", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("RoosterDefaultId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("RoosterDefaultId");

                    b.ToTable("RoosterTraining");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Database.Models.DbUsers", b =>
                {
                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", "Customer")
                        .WithMany("Users")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbAudit", b =>
                {
                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", "Customer")
                        .WithMany("Audits")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Drogecode.Knrm.Oefenrooster.Database.Models.DbUsers", "User")
                        .WithMany("Audits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterAvailable", b =>
                {
                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", "Customer")
                        .WithMany("RoosterAvailables")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterTraining", "Training")
                        .WithMany("RoosterAvailables")
                        .HasForeignKey("TrainingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Drogecode.Knrm.Oefenrooster.Database.Models.DbUsers", "User")
                        .WithMany("RoosterAvailables")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Training");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterDefault", b =>
                {
                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", "Customer")
                        .WithMany("RoosterDefaults")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterTraining", b =>
                {
                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", "Customer")
                        .WithMany("RoosterTraining")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterDefault", "RoosterDefault")
                        .WithMany("RoosterTrainings")
                        .HasForeignKey("RoosterDefaultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("RoosterDefault");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Database.Models.DbUsers", b =>
                {
                    b.Navigation("Audits");

                    b.Navigation("RoosterAvailables");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbCustomers", b =>
                {
                    b.Navigation("Audits");

                    b.Navigation("RoosterAvailables");

                    b.Navigation("RoosterDefaults");

                    b.Navigation("RoosterTraining");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterDefault", b =>
                {
                    b.Navigation("RoosterTrainings");
                });

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Server.Database.Models.DbRoosterTraining", b =>
                {
                    b.Navigation("RoosterAvailables");
                });
#pragma warning restore 612, 618
        }
    }
}