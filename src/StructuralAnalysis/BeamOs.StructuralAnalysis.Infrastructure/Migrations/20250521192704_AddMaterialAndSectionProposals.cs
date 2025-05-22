using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialAndSectionProposals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialProposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    ModulusOfElasticity = table.Column<double>(type: "double precision", nullable: false),
                    ModulusOfRigidity = table.Column<double>(type: "double precision", nullable: false),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    ExistingModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialProposal", x => new { x.Id, x.ModelProposalId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_MaterialProposal_Materials_ExistingId_ExistingModelId",
                        columns: x => new { x.ExistingId, x.ExistingModelId },
                        principalTable: "Materials",
                        principalColumns: new[] { "Id", "ModelId" });
                    table.ForeignKey(
                        name: "FK_MaterialProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionProfileProposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    StrongAxisMomentOfInertia = table.Column<double>(type: "double precision", nullable: false),
                    WeakAxisMomentOfInertia = table.Column<double>(type: "double precision", nullable: false),
                    PolarMomentOfInertia = table.Column<double>(type: "double precision", nullable: false),
                    StrongAxisPlasticSectionModulus = table.Column<double>(type: "double precision", nullable: false),
                    WeakAxisPlasticSectionModulus = table.Column<double>(type: "double precision", nullable: false),
                    StrongAxisShearArea = table.Column<double>(type: "double precision", nullable: true),
                    WeakAxisShearArea = table.Column<double>(type: "double precision", nullable: true),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    ExistingModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfileProposal", x => new { x.Id, x.ModelProposalId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_SectionProfileProposal_ModelProposals_ModelProposalId_Model~",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionProfileProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionProfileProposal_SectionProfileInfoBase_ExistingId_Ex~",
                        columns: x => new { x.ExistingId, x.ExistingModelId },
                        principalTable: "SectionProfileInfoBase",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateTable(
                name: "SectionProfileProposalFromLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    Library = table.Column<int>(type: "integer", nullable: false),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    ExistingModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfileProposalFromLibrary", x => new { x.Id, x.ModelProposalId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_SectionProfileProposalFromLibrary_ModelProposals_ModelPropo~",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionProfileProposalFromLibrary_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionProfileProposalFromLibrary_SectionProfileInfoBase_Ex~",
                        columns: x => new { x.ExistingId, x.ExistingModelId },
                        principalTable: "SectionProfileInfoBase",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProposal_ExistingId_ExistingModelId",
                table: "MaterialProposal",
                columns: new[] { "ExistingId", "ExistingModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProposal_ModelId",
                table: "MaterialProposal",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProposal_ModelProposalId_ModelId",
                table: "MaterialProposal",
                columns: new[] { "ModelProposalId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposal_ExistingId_ExistingModelId",
                table: "SectionProfileProposal",
                columns: new[] { "ExistingId", "ExistingModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposal_ModelId",
                table: "SectionProfileProposal",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposal_ModelProposalId_ModelId",
                table: "SectionProfileProposal",
                columns: new[] { "ModelProposalId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposalFromLibrary_ExistingId_ExistingModelId",
                table: "SectionProfileProposalFromLibrary",
                columns: new[] { "ExistingId", "ExistingModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposalFromLibrary_ModelId",
                table: "SectionProfileProposalFromLibrary",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposalFromLibrary_ModelProposalId_ModelId",
                table: "SectionProfileProposalFromLibrary",
                columns: new[] { "ModelProposalId", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialProposal");

            migrationBuilder.DropTable(
                name: "SectionProfileProposal");

            migrationBuilder.DropTable(
                name: "SectionProfileProposalFromLibrary");
        }
    }
}
