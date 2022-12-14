﻿// <auto-generated />
using System;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221211110052_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Drogecode.Knrm.Oefenrooster.Database.Models.DbUsers", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<DateTime>("Created")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("text");

                b.HasKey("Id");

                b.ToTable("Users");

                b.HasData(
                    new
                    {
                        Id = new Guid("46a4ddb6-412b-4329-b48f-ed681c96bc26"),
                        Created = new DateTime(2022, 12, 14, 19, 22, 16, 829, DateTimeKind.Utc).AddTicks(327),
                        Email = "test@drogecode.nl",
                        Name = "from model creating"
                    });
            });
#pragma warning restore 612, 618
        }
    }
}
