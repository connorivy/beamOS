using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalIssueCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "ProposalIssue",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "ProposalIssue");
        }
    }
}
