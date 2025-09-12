using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    internal partial class GoBackToCompositeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1dProposal_ModelProposals_ModelProposalId_ModelId",
                table: "Element1dProposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1dProposal_Models_ModelId",
                table: "Element1dProposal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Element1dProposal",
                table: "Element1dProposal");

            migrationBuilder.RenameTable(
                name: "Element1dProposal",
                newName: "Element1dProposals");

            migrationBuilder.RenameIndex(
                name: "IX_Element1dProposal_ModelProposalId_ModelId",
                table: "Element1dProposals",
                newName: "IX_Element1dProposals_ModelProposalId_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Element1dProposal_ModelId",
                table: "Element1dProposals",
                newName: "IX_Element1dProposals_ModelId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NodeResults",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Element1dResults",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Element1dProposals",
                table: "Element1dProposals",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1dProposals_ModelProposals_ModelProposalId_ModelId",
                table: "Element1dProposals",
                columns: new[] { "ModelProposalId", "ModelId" },
                principalTable: "ModelProposals",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Element1dProposals_Models_ModelId",
                table: "Element1dProposals",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1dProposals_ModelProposals_ModelProposalId_ModelId",
                table: "Element1dProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1dProposals_Models_ModelId",
                table: "Element1dProposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Element1dProposals",
                table: "Element1dProposals");

            migrationBuilder.RenameTable(
                name: "Element1dProposals",
                newName: "Element1dProposal");

            migrationBuilder.RenameIndex(
                name: "IX_Element1dProposals_ModelProposalId_ModelId",
                table: "Element1dProposal",
                newName: "IX_Element1dProposal_ModelProposalId_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Element1dProposals_ModelId",
                table: "Element1dProposal",
                newName: "IX_Element1dProposal_ModelId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NodeResults",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Element1dResults",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Element1dProposal",
                table: "Element1dProposal",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1dProposal_ModelProposals_ModelProposalId_ModelId",
                table: "Element1dProposal",
                columns: new[] { "ModelProposalId", "ModelId" },
                principalTable: "ModelProposals",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Element1dProposal_Models_ModelId",
                table: "Element1dProposal",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
