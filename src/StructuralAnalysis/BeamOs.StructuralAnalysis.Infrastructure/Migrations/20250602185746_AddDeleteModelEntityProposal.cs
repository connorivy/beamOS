using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeleteModelEntityProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelEntityDeleteProposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelEntityId = table.Column<int>(type: "integer", nullable: false),
                    ObjectType = table.Column<int>(type: "integer", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelEntityDeleteProposal", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_ModelEntityDeleteProposal_ModelProposals_ModelProposalId_Mo~",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelEntityDeleteProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelEntityDeleteProposal_ModelId",
                table: "ModelEntityDeleteProposal",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelEntityDeleteProposal_ModelProposalId_ModelId",
                table: "ModelEntityDeleteProposal",
                columns: new[] { "ModelProposalId", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelEntityDeleteProposal");
        }
    }
}
