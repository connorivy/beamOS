using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoadCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoadCombinationId1",
                table: "ResultSets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LoadCombinationModelId",
                table: "ResultSets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoadCaseId",
                table: "PointLoads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoadCaseId",
                table: "MomentLoads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LoadCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadCases", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_LoadCases_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoadCombinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LoadCaseFactors = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadCombinations", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_LoadCombinations_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultSets_LoadCombinationId1_LoadCombinationModelId",
                table: "ResultSets",
                columns: new[] { "LoadCombinationId1", "LoadCombinationModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_LoadCases_ModelId",
                table: "LoadCases",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadCombinations_ModelId",
                table: "LoadCombinations",
                column: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultSets_LoadCombinations_LoadCombinationId1_LoadCombinat~",
                table: "ResultSets",
                columns: new[] { "LoadCombinationId1", "LoadCombinationModelId" },
                principalTable: "LoadCombinations",
                principalColumns: new[] { "Id", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultSets_LoadCombinations_LoadCombinationId1_LoadCombinat~",
                table: "ResultSets");

            migrationBuilder.DropTable(
                name: "LoadCases");

            migrationBuilder.DropTable(
                name: "LoadCombinations");

            migrationBuilder.DropIndex(
                name: "IX_ResultSets_LoadCombinationId1_LoadCombinationModelId",
                table: "ResultSets");

            migrationBuilder.DropColumn(
                name: "LoadCombinationId1",
                table: "ResultSets");

            migrationBuilder.DropColumn(
                name: "LoadCombinationModelId",
                table: "ResultSets");

            migrationBuilder.DropColumn(
                name: "LoadCaseId",
                table: "PointLoads");

            migrationBuilder.DropColumn(
                name: "LoadCaseId",
                table: "MomentLoads");
        }
    }
}
