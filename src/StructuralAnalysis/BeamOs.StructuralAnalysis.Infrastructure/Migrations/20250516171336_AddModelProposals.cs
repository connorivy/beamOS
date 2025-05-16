using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelProposals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelProposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelChangeRequestId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_ModelProposals", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_ModelProposals_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Element1dProposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    EndNodeId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    EndNodeId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    MaterialId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    MaterialId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    SectionProfileId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    SectionProfileId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    StartNodeId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    StartNodeId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    ExistingModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1dProposal", x => new { x.Id, x.ModelProposalId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Element1dProposal_Element1ds_ExistingId_ExistingModelId",
                        columns: x => new { x.ExistingId, x.ExistingModelId },
                        principalTable: "Element1ds",
                        principalColumns: new[] { "Id", "ModelId" });
                    table.ForeignKey(
                        name: "FK_Element1dProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Element1dProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeProposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    LocationPoint_X = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Y = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Z = table.Column<double>(type: "double precision", nullable: false),
                    Restraint_CanRotateAboutX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutZ = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(type: "boolean", nullable: false),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    ExistingModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeProposal", x => new { x.Id, x.ModelProposalId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_NodeProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NodeProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                        columns: x => new { x.ExistingId, x.ExistingModelId },
                        principalTable: "Nodes",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Element1dProposal_ExistingId_ExistingModelId",
                table: "Element1dProposal",
                columns: new[] { "ExistingId", "ExistingModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Element1dProposal_ModelId",
                table: "Element1dProposal",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Element1dProposal_ModelProposalId_ModelId",
                table: "Element1dProposal",
                columns: new[] { "ModelProposalId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_ModelProposals_ModelId",
                table: "ModelProposals",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeProposal_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_NodeProposal_ModelId",
                table: "NodeProposal",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeProposal_ModelProposalId_ModelId",
                table: "NodeProposal",
                columns: new[] { "ModelProposalId", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Element1dProposal");

            migrationBuilder.DropTable(
                name: "NodeProposal");

            migrationBuilder.DropTable(
                name: "ModelProposals");
        }
    }
}
