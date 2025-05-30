using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dontmapnodeelemetns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_NodeId_NodeModelId",
                table: "Element1ds");

            migrationBuilder.DropIndex(
                name: "IX_Element1ds_NodeId_NodeModelId",
                table: "Element1ds");

            migrationBuilder.DropColumn(
                name: "NodeId",
                table: "Element1ds");

            migrationBuilder.DropColumn(
                name: "NodeModelId",
                table: "Element1ds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NodeId",
                table: "Element1ds",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NodeModelId",
                table: "Element1ds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_NodeId_NodeModelId",
                table: "Element1ds",
                columns: new[] { "NodeId", "NodeModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_NodeId_NodeModelId",
                table: "Element1ds",
                columns: new[] { "NodeId", "NodeModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });
        }
    }
}
