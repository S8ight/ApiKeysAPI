﻿// <auto-generated />
using System;
using ApiKeysApi.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiKeysApi.Migrations
{
    [DbContext(typeof(ApiKeysDbContext))]
    [Migration("20240426130328_InitMigrations")]
    partial class InitMigrations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApiKeysApi.DataAccess.Entities.ApiKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccessIpWhitelist")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApiKeyHash")
                        .IsRequired()
                        .HasColumnType("VARCHAR(255)");

                    b.Property<string>("ApiKeyName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(255)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Environment")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FailedAttempts")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<DateTime?>("LastUsedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<int>("RateLimitDay")
                        .HasColumnType("int");

                    b.Property<int>("RateLimitHour")
                        .HasColumnType("int");

                    b.Property<int>("RateLimitMinute")
                        .HasColumnType("int");

                    b.Property<int>("RateLimitSecond")
                        .HasColumnType("int");

                    b.Property<string>("RevocationReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("ApiKeysApi.DataAccess.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Password = "$2a$12$mBXrsuF73vv6qFovuE8tEOvBhABgHgcvgoTJU8lHoY3UgMgT7O622",
                            Role = 1,
                            UserName = "OrdinaryUser"
                        });
                });

            modelBuilder.Entity("ApiKeysApi.DataAccess.Entities.ApiKey", b =>
                {
                    b.HasOne("ApiKeysApi.DataAccess.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}