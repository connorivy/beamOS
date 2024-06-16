﻿// <auto-generated />
using System;
using System.Collections.Generic;
using BeamOs.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BeamOs.Infrastructure.Migrations
{
    [DbContext(typeof(BeamOsStructuralDbContext))]
    partial class BeamOsStructuralDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BeamOs.Domain.AnalyticalResults.NodeResultAggregate.NodeResult", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("NodeId")
                        .HasColumnType("uniqueidentifier");

                    b.ComplexProperty<Dictionary<string, object>>("Displacements", "BeamOs.Domain.AnalyticalResults.NodeResultAggregate.NodeResult.Displacements#Displacements", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<double>("DisplacementAlongX")
                                .HasColumnType("float");

                            b1.Property<double>("DisplacementAlongY")
                                .HasColumnType("float");

                            b1.Property<double>("DisplacementAlongZ")
                                .HasColumnType("float");

                            b1.Property<double>("RotationAboutX")
                                .HasColumnType("float");

                            b1.Property<double>("RotationAboutY")
                                .HasColumnType("float");

                            b1.Property<double>("RotationAboutZ")
                                .HasColumnType("float");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Forces", "BeamOs.Domain.AnalyticalResults.NodeResultAggregate.NodeResult.Forces#Forces", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<double>("ForceAlongX")
                                .HasColumnType("float");

                            b1.Property<double>("ForceAlongY")
                                .HasColumnType("float");

                            b1.Property<double>("ForceAlongZ")
                                .HasColumnType("float");

                            b1.Property<double>("MomentAboutX")
                                .HasColumnType("float");

                            b1.Property<double>("MomentAboutY")
                                .HasColumnType("float");

                            b1.Property<double>("MomentAboutZ")
                                .HasColumnType("float");
                        });

                    b.HasKey("Id");

                    b.HasIndex("NodeId")
                        .IsUnique();

                    b.ToTable("NodeResults");
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.Element1DAggregate.Element1D", b =>
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

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.MaterialAggregate.Material", b =>
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

                    b.HasIndex("ModelId");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.ModelAggregate.Model", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ComplexProperty<Dictionary<string, object>>("Settings", "BeamOs.Domain.PhysicalModel.ModelAggregate.Model.Settings#ModelSettings", b1 =>
                        {
                            b1.IsRequired();

                            b1.ComplexProperty<Dictionary<string, object>>("UnitSettings", "BeamOs.Domain.PhysicalModel.ModelAggregate.Model.Settings#ModelSettings.UnitSettings#UnitSettings", b2 =>
                                {
                                    b2.IsRequired();

                                    b2.Property<int>("AngleUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("AreaMomentOfInertiaUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("AreaUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("ForcePerLengthUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("ForceUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("LengthUnit")
                                        .HasColumnType("int");

                                    b2.Property<int>("PressureUnit")
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

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.MomentLoadAggregate.MomentLoad", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AxisDirection")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("NodeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Torque")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("MomentLoads");
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.NodeAggregate.Node", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.ComplexProperty<Dictionary<string, object>>("LocationPoint", "BeamOs.Domain.PhysicalModel.NodeAggregate.Node.LocationPoint#Point", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<double>("XCoordinate")
                                .HasColumnType("float");

                            b1.Property<double>("YCoordinate")
                                .HasColumnType("float");

                            b1.Property<double>("ZCoordinate")
                                .HasColumnType("float");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Restraint", "BeamOs.Domain.PhysicalModel.NodeAggregate.Node.Restraint#Restraint", b1 =>
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

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.PointLoadAggregate.PointLoad", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Direction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Force")
                        .HasColumnType("float");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("NodeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("PointLoads");
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.SectionProfileAggregate.SectionProfile", b =>
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

                    b.HasIndex("ModelId");

                    b.ToTable("SectionProfiles");
                });

            modelBuilder.Entity("BeamOs.Domain.AnalyticalResults.NodeResultAggregate.NodeResult", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.NodeAggregate.Node", null)
                        .WithOne()
                        .HasForeignKey("BeamOs.Domain.AnalyticalResults.NodeResultAggregate.NodeResult", "NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.Element1DAggregate.Element1D", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.ModelAggregate.Model", null)
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.MaterialAggregate.Material", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.ModelAggregate.Model", null)
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.MomentLoadAggregate.MomentLoad", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.NodeAggregate.Node", null)
                        .WithMany()
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.NodeAggregate.Node", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.ModelAggregate.Model", null)
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.PointLoadAggregate.PointLoad", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.NodeAggregate.Node", null)
                        .WithMany()
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeamOs.Domain.PhysicalModel.SectionProfileAggregate.SectionProfile", b =>
                {
                    b.HasOne("BeamOs.Domain.PhysicalModel.ModelAggregate.Model", null)
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
