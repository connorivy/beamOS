using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoadCombRemoveName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "LoadCombinations");

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_LoadCaseId_ModelId",
                table: "PointLoads",
                columns: new[] { "LoadCaseId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_LoadCaseId_ModelId",
                table: "MomentLoads",
                columns: new[] { "LoadCaseId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MomentLoads_LoadCases_LoadCaseId_ModelId",
                table: "MomentLoads",
                columns: new[] { "LoadCaseId", "ModelId" },
                principalTable: "LoadCases",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                table: "PointLoads",
                columns: new[] { "LoadCaseId", "ModelId" },
                principalTable: "LoadCases",
                principalColumns: new[] { "Id", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MomentLoads_LoadCases_LoadCaseId_ModelId",
                table: "MomentLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropIndex(
                name: "IX_PointLoads_LoadCaseId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropIndex(
                name: "IX_MomentLoads_LoadCaseId_ModelId",
                table: "MomentLoads");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LoadCombinations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
