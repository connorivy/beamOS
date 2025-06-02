using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectModelEntityDeleteProposalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelEntityDeleteProposal",
                table: "ModelEntityDeleteProposal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelEntityDeleteProposal",
                table: "ModelEntityDeleteProposal",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelEntityDeleteProposal",
                table: "ModelEntityDeleteProposal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelEntityDeleteProposal",
                table: "ModelEntityDeleteProposal",
                columns: new[] { "Id", "ModelId" });
        }
    }
}
