using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Settings_YAxisUp = table.Column<bool>(type: "boolean", nullable: false),
                    Settings_AnalysisSettings_Element1DAnalysisType = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_AngleUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_AreaMomentOfInertiaUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_AreaUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_ForcePerLengthUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_ForceUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_LengthUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_PressureUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_TorqueUnit = table.Column<int>(type: "integer", nullable: false),
                    Settings_UnitSettings_VolumeUnit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModulusOfElasticity = table.Column<double>(type: "double precision", nullable: false),
                    ModulusOfRigidity = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Materials_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationPoint_X = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Y = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Z = table.Column<double>(type: "double precision", nullable: false),
                    Restraint_CanRotateAboutX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutZ = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Nodes_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultSets", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_ResultSets_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    StrongAxisMomentOfInertia = table.Column<double>(type: "double precision", nullable: false),
                    WeakAxisMomentOfInertia = table.Column<double>(type: "double precision", nullable: false),
                    PolarMomentOfInertia = table.Column<double>(type: "double precision", nullable: false),
                    StrongAxisShearArea = table.Column<double>(type: "double precision", nullable: false),
                    WeakAxisShearArea = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfiles", x => new { x.Id, x.ModelId });
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
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    Torque = table.Column<double>(type: "double precision", nullable: false),
                    AxisDirection_X = table.Column<double>(type: "double precision", nullable: false),
                    AxisDirection_Y = table.Column<double>(type: "double precision", nullable: false),
                    AxisDirection_Z = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomentLoads", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_MomentLoads_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MomentLoads_Nodes_NodeId_ModelId",
                        columns: x => new { x.NodeId, x.ModelId },
                        principalTable: "Nodes",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateTable(
                name: "PointLoads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    Force = table.Column<double>(type: "double precision", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointLoads", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_PointLoads_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointLoads_Nodes_NodeId_ModelId",
                        columns: x => new { x.NodeId, x.ModelId },
                        principalTable: "Nodes",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateTable(
                name: "Element1dResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxShear = table.Column<double>(type: "double precision", nullable: false),
                    MinShear = table.Column<double>(type: "double precision", nullable: false),
                    MaxMoment = table.Column<double>(type: "double precision", nullable: false),
                    MinMoment = table.Column<double>(type: "double precision", nullable: false),
                    MaxDisplacement = table.Column<double>(type: "double precision", nullable: false),
                    MinDisplacement = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1dResults", x => new { x.Id, x.ResultSetId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Element1dResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Element1dResults_ResultSets_ResultSetId_ModelId",
                        columns: x => new { x.ResultSetId, x.ModelId },
                        principalTable: "ResultSets",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    Displacements_DisplacementAlongX = table.Column<double>(type: "double precision", nullable: false),
                    Displacements_DisplacementAlongY = table.Column<double>(type: "double precision", nullable: false),
                    Displacements_DisplacementAlongZ = table.Column<double>(type: "double precision", nullable: false),
                    Displacements_RotationAboutX = table.Column<double>(type: "double precision", nullable: false),
                    Displacements_RotationAboutY = table.Column<double>(type: "double precision", nullable: false),
                    Displacements_RotationAboutZ = table.Column<double>(type: "double precision", nullable: false),
                    Forces_ForceAlongX = table.Column<double>(type: "double precision", nullable: false),
                    Forces_ForceAlongY = table.Column<double>(type: "double precision", nullable: false),
                    Forces_ForceAlongZ = table.Column<double>(type: "double precision", nullable: false),
                    Forces_MomentAboutX = table.Column<double>(type: "double precision", nullable: false),
                    Forces_MomentAboutY = table.Column<double>(type: "double precision", nullable: false),
                    Forces_MomentAboutZ = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeResults", x => new { x.Id, x.ResultSetId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_NodeResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NodeResults_ResultSets_ResultSetId_ModelId",
                        columns: x => new { x.ResultSetId, x.ModelId },
                        principalTable: "ResultSets",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Element1ds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartNodeId = table.Column<int>(type: "integer", nullable: false),
                    EndNodeId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    SectionProfileId = table.Column<int>(type: "integer", nullable: false),
                    SectionProfileRotation = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1ds", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Element1ds_Materials_MaterialId_ModelId",
                        columns: x => new { x.MaterialId, x.ModelId },
                        principalTable: "Materials",
                        principalColumns: new[] { "Id", "ModelId" });
                    table.ForeignKey(
                        name: "FK_Element1ds_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                        columns: x => new { x.EndNodeId, x.ModelId },
                        principalTable: "Nodes",
                        principalColumns: new[] { "Id", "ModelId" });
                    table.ForeignKey(
                        name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
                        columns: x => new { x.StartNodeId, x.ModelId },
                        principalTable: "Nodes",
                        principalColumns: new[] { "Id", "ModelId" });
                    table.ForeignKey(
                        name: "FK_Element1ds_SectionProfiles_SectionProfileId_ModelId",
                        columns: x => new { x.SectionProfileId, x.ModelId },
                        principalTable: "SectionProfiles",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Element1dResults_ModelId",
                table: "Element1dResults",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1dResults_ResultSetId_ModelId",
                table: "Element1dResults",
                columns: new[] { "ResultSetId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_MaterialId_ModelId",
                table: "Element1ds",
                columns: new[] { "MaterialId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_ModelId",
                table: "Element1ds",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_SectionProfileId_ModelId",
                table: "Element1ds",
                columns: new[] { "SectionProfileId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ModelId",
                table: "Materials",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_ModelId",
                table: "MomentLoads",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_NodeId_ModelId",
                table: "MomentLoads",
                columns: new[] { "NodeId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_NodeResults_ModelId",
                table: "NodeResults",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeResults_ResultSetId_ModelId",
                table: "NodeResults",
                columns: new[] { "ResultSetId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId",
                table: "Nodes",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_ModelId",
                table: "PointLoads",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultSets_ModelId",
                table: "ResultSets",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfiles_ModelId",
                table: "SectionProfiles",
                column: "ModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Element1dResults");

            migrationBuilder.DropTable(
                name: "Element1ds");

            migrationBuilder.DropTable(
                name: "MomentLoads");

            migrationBuilder.DropTable(
                name: "NodeResults");

            migrationBuilder.DropTable(
                name: "PointLoads");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "SectionProfiles");

            migrationBuilder.DropTable(
                name: "ResultSets");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "Models");
        }
    }
}
