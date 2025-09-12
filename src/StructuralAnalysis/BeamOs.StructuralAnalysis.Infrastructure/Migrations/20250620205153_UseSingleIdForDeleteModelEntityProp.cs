using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    internal partial class UseSingleIdForDeleteModelEntityProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });
        }
    }
}
