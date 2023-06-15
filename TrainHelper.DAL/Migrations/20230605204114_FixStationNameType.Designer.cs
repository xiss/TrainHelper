﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrainHelper.DAL;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230605204114_FixStationNameType")]
    partial class FixStationNameType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TrainHelper.DAL.Entities.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CarNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FreightEtsngName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FreightTotalWeightKg")
                        .HasColumnType("integer");

                    b.Property<int>("TrainId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrainId");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("TrainHelper.DAL.Entities.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("InvoiceNum")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("TrainHelper.DAL.Entities.Station", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("StationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Stations");
                });

            modelBuilder.Entity("TrainHelper.DAL.Entities.Train", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FromStationId")
                        .HasColumnType("integer");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("integer");

                    b.Property<string>("LastOperationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LastStationId")
                        .HasColumnType("integer");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("ToStationId")
                        .HasColumnType("integer");

                    b.Property<string>("TrainIndexCombined")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("WhenLastOperation")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FromStationId");

                    b.HasIndex("InvoiceId");

                    b.HasIndex("LastStationId");

                    b.HasIndex("ToStationId");

                    b.ToTable("Trains");
                });

            modelBuilder.Entity("TrainHelper.DAL.Entities.Car", b =>
                {
                    b.HasOne("TrainHelper.DAL.Entities.Train", "Train")
                        .WithMany("Cars")
                        .HasForeignKey("TrainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Train");
                });

            modelBuilder.Entity("TrainHelper.DAL.Entities.Train", b =>
                {
                    b.HasOne("TrainHelper.DAL.Entities.Station", "FromStation")
                        .WithMany()
                        .HasForeignKey("FromStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrainHelper.DAL.Entities.Invoice", "Invoice")
                        .WithMany()
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrainHelper.DAL.Entities.Station", "LastStation")
                        .WithMany()
                        .HasForeignKey("LastStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrainHelper.DAL.Entities.Station", "ToStation")
                        .WithMany()
                        .HasForeignKey("ToStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromStation");

                    b.Navigation("Invoice");

                    b.Navigation("LastStation");

                    b.Navigation("ToStation");
                });

            modelBuilder.Entity("TrainHelper.DAL.Entities.Train", b =>
                {
                    b.Navigation("Cars");
                });
#pragma warning restore 612, 618
        }
    }
}
