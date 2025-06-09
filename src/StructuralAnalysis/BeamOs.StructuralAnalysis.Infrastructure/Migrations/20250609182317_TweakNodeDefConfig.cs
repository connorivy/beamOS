using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TweakNodeDefConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads");

            migrationBuilder.AddColumn<Guid>(
                name: "InternalNode_ModelId1",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_InternalNode_ModelId1",
                table: "Nodes",
                column: "InternalNode_ModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes",
                column: "ModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Models_InternalNode_ModelId1",
                table: "Nodes",
                column: "InternalNode_ModelId1",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                table: "PointLoads",
                columns: new[] { "LoadCaseId", "ModelId" },
                principalTable: "LoadCases",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_InternalNode_ModelId1",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_InternalNode_ModelId1",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNode_ModelId1",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "Nodes");

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                table: "PointLoads",
                columns: new[] { "LoadCaseId", "ModelId" },
                principalTable: "LoadCases",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });
        }
    }
}
