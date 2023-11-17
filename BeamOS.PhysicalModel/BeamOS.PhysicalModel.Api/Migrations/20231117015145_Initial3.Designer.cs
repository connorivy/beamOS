﻿// <auto-generated />
using System;
using System.Collections.Generic;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BeamOS.PhysicalModel.Api.Migrations
{
    [DbContext(typeof(PhysicalModelDbContext))]
    [Migration("20231117015145_Initial3")]
    partial class Initial3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.Element1DAggregate.Element1D", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EndNodeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MaterialId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SectionProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("SectionProfileRotation")
                        .HasColumnType("float");

                    b.Property<Guid>("StartNodeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Element1Ds");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.NodeAggregate.Node", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.ComplexProperty<Dictionary<string, object>>("LocationPoint", "BeamOS.PhysicalModel.Domain.NodeAggregate.Node.LocationPoint#Point", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<double>("XCoordinate")
                                .HasColumnType("float");

                            b1.Property<double>("YCoordinate")
                                .HasColumnType("float");

                            b1.Property<double>("ZCoordinate")
                                .HasColumnType("float");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Restraints", "BeamOS.PhysicalModel.Domain.NodeAggregate.Node.Restraints#Restraints", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<bool>("CanRotateAboutX")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanRotateAboutY")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanRotateAboutZ")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanTranslateAlongX")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanTranslateAlongY")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanTranslateAlongZ")
                                .HasColumnType("bit");
                        });

                    b.HasKey("Id");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.PointLoadAggregate.PointLoad", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Force")
                        .HasColumnType("float");

                    b.Property<Guid>("NodeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NormalizedDirection")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("PointLoad");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.PointLoadAggregate.PointLoad", b =>
                {
                    b.HasOne("BeamOS.PhysicalModel.Domain.NodeAggregate.Node", null)
                        .WithMany()
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
