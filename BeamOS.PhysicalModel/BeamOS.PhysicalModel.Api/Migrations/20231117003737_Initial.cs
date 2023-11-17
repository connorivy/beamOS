using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOS.PhysicalModel.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Element1Ds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionProfileRotation = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1Ds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationPoint_XCoordinate = table.Column<double>(type: "float", nullable: false),
                    LocationPoint_YCoordinate = table.Column<double>(type: "float", nullable: false),
                    LocationPoint_ZCoordinate = table.Column<double>(type: "float", nullable: false),
                    Restraints_CanRotateAboutX = table.Column<bool>(type: "bit", nullable: false),
                    Restraints_CanRotateAboutY = table.Column<bool>(type: "bit", nullable: false),
                    Restraints_CanRotateAboutZ = table.Column<bool>(type: "bit", nullable: false),
                    Restraints_CanTranslateAlongX = table.Column<bool>(type: "bit", nullable: false),
                    Restraints_CanTranslateAlongY = table.Column<bool>(type: "bit", nullable: false),
                    Restraints_CanTranslateAlongZ = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointLoad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Force = table.Column<double>(type: "float", nullable: false),
                    NormalizedDirection = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointLoad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointLoad_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointLoad_NodeId",
                table: "PointLoad",
                column: "NodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Element1Ds");

            migrationBuilder.DropTable(
                name: "PointLoad");

            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
