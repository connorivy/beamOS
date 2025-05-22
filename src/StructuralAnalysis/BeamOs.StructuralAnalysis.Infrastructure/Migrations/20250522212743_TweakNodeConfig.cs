using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TweakNodeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
                table: "Element1ds");

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
                name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_NodeId_NodeModelId",
                table: "Element1ds",
                columns: new[] { "NodeId", "NodeModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_NodeId_NodeModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });
        }
    }
}
