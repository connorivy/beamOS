using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseTpc3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternalNodes_Models_ModelId1",
                table: "InternalNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_InternalNodes_ModelId1",
                table: "InternalNodes");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "InternalNodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "InternalNodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes",
                column: "ModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_InternalNodes_ModelId1",
                table: "InternalNodes",
                column: "ModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNodes_Models_ModelId1",
                table: "InternalNodes",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");
        }
    }
}
