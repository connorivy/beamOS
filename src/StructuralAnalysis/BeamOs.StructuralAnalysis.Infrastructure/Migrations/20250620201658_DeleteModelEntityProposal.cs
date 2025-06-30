using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteModelEntityProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeleteModelEntityProposal_ModelProposals_ModelProposalId_Mo~",
                table: "DeleteModelEntityProposal");

            migrationBuilder.DropForeignKey(
                name: "FK_DeleteModelEntityProposal_Models_ModelId",
                table: "DeleteModelEntityProposal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposal",
                table: "DeleteModelEntityProposal");

            migrationBuilder.RenameTable(
                name: "DeleteModelEntityProposal",
                newName: "DeleteModelEntityProposals");

            migrationBuilder.RenameIndex(
                name: "IX_DeleteModelEntityProposal_ModelProposalId_ModelId",
                table: "DeleteModelEntityProposals",
                newName: "IX_DeleteModelEntityProposals_ModelProposalId_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_DeleteModelEntityProposal_ModelId",
                table: "DeleteModelEntityProposals",
                newName: "IX_DeleteModelEntityProposals_ModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DeleteModelEntityProposals_ModelProposals_ModelProposalId_M~",
                table: "DeleteModelEntityProposals",
                columns: new[] { "ModelProposalId", "ModelId" },
                principalTable: "ModelProposals",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeleteModelEntityProposals_Models_ModelId",
                table: "DeleteModelEntityProposals",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeleteModelEntityProposals_ModelProposals_ModelProposalId_M~",
                table: "DeleteModelEntityProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_DeleteModelEntityProposals_Models_ModelId",
                table: "DeleteModelEntityProposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.RenameTable(
                name: "DeleteModelEntityProposals",
                newName: "DeleteModelEntityProposal");

            migrationBuilder.RenameIndex(
                name: "IX_DeleteModelEntityProposals_ModelProposalId_ModelId",
                table: "DeleteModelEntityProposal",
                newName: "IX_DeleteModelEntityProposal_ModelProposalId_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_DeleteModelEntityProposals_ModelId",
                table: "DeleteModelEntityProposal",
                newName: "IX_DeleteModelEntityProposal_ModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposal",
                table: "DeleteModelEntityProposal",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DeleteModelEntityProposal_ModelProposals_ModelProposalId_Mo~",
                table: "DeleteModelEntityProposal",
                columns: new[] { "ModelProposalId", "ModelId" },
                principalTable: "ModelProposals",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeleteModelEntityProposal_Models_ModelId",
                table: "DeleteModelEntityProposal",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
