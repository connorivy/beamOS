using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Settings_YAxisUp = table.Column<bool>(type: "bit", nullable: false),
                    Settings_AnalysisSettings_Element1DAnalysisType = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_AngleUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_AreaMomentOfInertiaUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_AreaUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_ForcePerLengthUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_ForceUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_LengthUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_PressureUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_TorqueUnit = table.Column<int>(type: "int", nullable: false),
                    Settings_UnitSettings_VolumeUnit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MomentDiagrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Element1DId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShearDirection = table.Column<int>(type: "int", nullable: false),
                    GlobalShearDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElementLength = table.Column<double>(type: "float", nullable: false),
                    EqualityTolerance = table.Column<double>(type: "float", nullable: false),
                    LengthUnit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomentDiagrams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModulusOfElasticity = table.Column<double>(type: "float", nullable: false),
                    ModulusOfRigidity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxShear = table.Column<double>(type: "float", nullable: false),
                    MinShear = table.Column<double>(type: "float", nullable: false),
                    MaxMoment = table.Column<double>(type: "float", nullable: false),
                    MinMoment = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationPoint_XCoordinate = table.Column<double>(type: "float", nullable: false),
                    LocationPoint_YCoordinate = table.Column<double>(type: "float", nullable: false),
                    LocationPoint_ZCoordinate = table.Column<double>(type: "float", nullable: false),
                    Restraint_CanRotateAboutX = table.Column<bool>(type: "bit", nullable: false),
                    Restraint_CanRotateAboutY = table.Column<bool>(type: "bit", nullable: false),
                    Restraint_CanRotateAboutZ = table.Column<bool>(type: "bit", nullable: false),
                    Restraint_CanTranslateAlongX = table.Column<bool>(type: "bit", nullable: false),
                    Restraint_CanTranslateAlongY = table.Column<bool>(type: "bit", nullable: false),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nodes_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: false),
                    StrongAxisMomentOfInertia = table.Column<double>(type: "float", nullable: false),
                    WeakAxisMomentOfInertia = table.Column<double>(type: "float", nullable: false),
                    PolarMomentOfInertia = table.Column<double>(type: "float", nullable: false),
                    StrongAxisShearArea = table.Column<double>(type: "float", nullable: false),
                    WeakAxisShearArea = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionProfiles_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MomentLoads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Torque = table.Column<double>(type: "float", nullable: false),
                    AxisDirection = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomentLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MomentLoads_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MomentLoads_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NodeResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Displacements_DisplacementAlongX = table.Column<double>(type: "float", nullable: false),
                    Displacements_DisplacementAlongY = table.Column<double>(type: "float", nullable: false),
                    Displacements_DisplacementAlongZ = table.Column<double>(type: "float", nullable: false),
                    Displacements_RotationAboutX = table.Column<double>(type: "float", nullable: false),
                    Displacements_RotationAboutY = table.Column<double>(type: "float", nullable: false),
                    Displacements_RotationAboutZ = table.Column<double>(type: "float", nullable: false),
                    Forces_ForceAlongX = table.Column<double>(type: "float", nullable: false),
                    Forces_ForceAlongY = table.Column<double>(type: "float", nullable: false),
                    Forces_ForceAlongZ = table.Column<double>(type: "float", nullable: false),
                    Forces_MomentAboutX = table.Column<double>(type: "float", nullable: false),
                    Forces_MomentAboutY = table.Column<double>(type: "float", nullable: false),
                    Forces_MomentAboutZ = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeResults_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointLoads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Force = table.Column<double>(type: "float", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointLoads_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointLoads_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Element1Ds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionProfileRotation = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1Ds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Element1Ds_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Element1Ds_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Element1Ds_Nodes_EndNodeId",
                        column: x => x.EndNodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Element1Ds_Nodes_StartNodeId",
                        column: x => x.StartNodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Element1Ds_SectionProfiles_SectionProfileId",
                        column: x => x.SectionProfileId,
                        principalTable: "SectionProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShearForceDiagrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Element1DId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShearDirection = table.Column<int>(type: "int", nullable: false),
                    GlobalShearDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElementLength = table.Column<double>(type: "float", nullable: false),
                    EqualityTolerance = table.Column<double>(type: "float", nullable: false),
                    LengthUnit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShearForceDiagrams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShearForceDiagrams_Element1Ds_Element1DId",
                        column: x => x.Element1DId,
                        principalTable: "Element1Ds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiagramConsistantIntervals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartLocation = table.Column<double>(type: "float", nullable: false),
                    EndLocation = table.Column<double>(type: "float", nullable: false),
                    Polynomial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LengthUnit = table.Column<int>(type: "int", nullable: false),
                    MomentDiagramId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShearForceDiagramId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagramConsistantIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagramConsistantIntervals_MomentDiagrams_MomentDiagramId",
                        column: x => x.MomentDiagramId,
                        principalTable: "MomentDiagrams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagramConsistantIntervals_ShearForceDiagrams_ShearForceDiagramId",
                        column: x => x.ShearForceDiagramId,
                        principalTable: "ShearForceDiagrams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagramConsistantIntervals_MomentDiagramId",
                table: "DiagramConsistantIntervals",
                column: "MomentDiagramId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagramConsistantIntervals_ShearForceDiagramId",
                table: "DiagramConsistantIntervals",
                column: "ShearForceDiagramId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1Ds_EndNodeId",
                table: "Element1Ds",
                column: "EndNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1Ds_MaterialId",
                table: "Element1Ds",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1Ds_ModelId",
                table: "Element1Ds",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1Ds_SectionProfileId",
                table: "Element1Ds",
                column: "SectionProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1Ds_StartNodeId",
                table: "Element1Ds",
                column: "StartNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ModelId",
                table: "Materials",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelResults_ModelId",
                table: "ModelResults",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_ModelId",
                table: "MomentLoads",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_NodeId",
                table: "MomentLoads",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeResults_NodeId",
                table: "NodeResults",
                column: "NodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId",
                table: "Nodes",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_ModelId",
                table: "PointLoads",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_NodeId",
                table: "PointLoads",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfiles_ModelId",
                table: "SectionProfiles",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ShearForceDiagrams_Element1DId",
                table: "ShearForceDiagrams",
                column: "Element1DId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagramConsistantIntervals");

            migrationBuilder.DropTable(
                name: "ModelResults");

            migrationBuilder.DropTable(
                name: "MomentLoads");

            migrationBuilder.DropTable(
                name: "NodeResults");

            migrationBuilder.DropTable(
                name: "PointLoads");

            migrationBuilder.DropTable(
                name: "MomentDiagrams");

            migrationBuilder.DropTable(
                name: "ShearForceDiagrams");

            migrationBuilder.DropTable(
                name: "Element1Ds");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "SectionProfiles");

            migrationBuilder.DropTable(
                name: "Models");
        }
    }
}
