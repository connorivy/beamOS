using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MomentDiagramId",
                table: "DiagramConsistantIntervals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MomentDiagrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Element1DId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShearDirection = table.Column<int>(type: "int", nullable: false),
                    GlobalShearDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElementLength = table.Column<double>(type: "float", nullable: false),
                    EqualityTolerance = table.Column<double>(type: "float", nullable: false),
                    LengthUnit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomentDiagrams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagramConsistantIntervals_MomentDiagramId",
                table: "DiagramConsistantIntervals",
                column: "MomentDiagramId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiagramConsistantIntervals_MomentDiagrams_MomentDiagramId",
                table: "DiagramConsistantIntervals",
                column: "MomentDiagramId",
                principalTable: "MomentDiagrams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiagramConsistantIntervals_MomentDiagrams_MomentDiagramId",
                table: "DiagramConsistantIntervals");

            migrationBuilder.DropTable(
                name: "MomentDiagrams");

            migrationBuilder.DropIndex(
                name: "IX_DiagramConsistantIntervals_MomentDiagramId",
                table: "DiagramConsistantIntervals");

            migrationBuilder.DropColumn(
                name: "MomentDiagramId",
                table: "DiagramConsistantIntervals");
        }
    }
}
