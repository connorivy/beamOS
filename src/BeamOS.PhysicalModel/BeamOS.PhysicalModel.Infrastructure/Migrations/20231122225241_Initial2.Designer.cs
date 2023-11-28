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

namespace BeamOS.PhysicalModel.Infrastructure.Migrations
{
    [DbContext(typeof(PhysicalModelDbContext))]
    [Migration("20231122225241_Initial2")]
    partial class Initial2
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

                    b.HasIndex("ModelId");

                    b.ToTable("Element1Ds");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.MaterialAggregate.Material", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("ModulusOfElasticity")
                        .HasColumnType("float");

                    b.Property<double>("ModulusOfRigidity")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.ModelAggregate.Model", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ComplexProperty<Dictionary<string, object>>("Settings", "BeamOS.PhysicalModel.Domain.ModelAggregate.Model.Settings#ModelSettings", b1 =>
                        {
                            b1.IsRequired();

                            b1.ComplexProperty<Dictionary<string, object>>("UnitSettings", "BeamOS.PhysicalModel.Domain.ModelAggregate.Model.Settings#ModelSettings.UnitSettings#UnitSettings", b2 =>
                                {
                                    b2.IsRequired();

                                    b2.Property<int>("AreaUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("ForcePerLengthUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("ForceUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("LengthUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("TorqueUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("VolumeUnit")
                                        .HasColumnType("int");
                                });
                        });

                    b.HasKey("Id");

                    b.ToTable("Models");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.NodeAggregate.Node", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ModelId")
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

                    b.ComplexProperty<Dictionary<string, object>>("Restraint", "BeamOS.PhysicalModel.Domain.NodeAggregate.Node.Restraint#Restraint", b1 =>
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

                    b.HasIndex("ModelId");

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

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.SectionProfileAggregate.SectionProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Area")
                        .HasColumnType("float");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("PolarMomentOfInertia")
                        .HasColumnType("float");

                    b.Property<double>("StrongAxisMomentOfInertia")
                        .HasColumnType("float");

                    b.Property<double>("WeakAxisMomentOfInertia")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("SectionProfiles");
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.Element1DAggregate.Element1D", b =>
                {
                    b.HasOne("BeamOS.PhysicalModel.Domain.ModelAggregate.Model", null)
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOS.PhysicalModel.Domain.NodeAggregate.Node", b =>
                {
                    b.HasOne("BeamOS.PhysicalModel.Domain.ModelAggregate.Model", null)
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
