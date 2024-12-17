using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationPoint_X = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Y = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Z = table.Column<double>(type: "double precision", nullable: false),
                    Restraint_CanRotateAboutX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutZ = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => new { x.Id, x.ModelId });
                });

            migrationBuilder.CreateTable(
                name: "PointLoad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    Force = table.Column<double>(type: "double precision", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: false),
                    NodeModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointLoad", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_PointLoad_Nodes_NodeId_NodeModelId",
                        columns: x => new { x.NodeId, x.NodeModelId },
                        principalTable: "Nodes",
                        principalColumns: new[] { "Id", "ModelId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointLoad_NodeId_NodeModelId",
                table: "PointLoad",
                columns: new[] { "NodeId", "NodeModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointLoad");

            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
