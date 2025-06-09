using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseTpc2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternalNode_Element1ds_Element1dId_ModelId",
                table: "InternalNode");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalNode_Models_ModelId",
                table: "InternalNode");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalNode_Models_ModelId1",
                table: "InternalNode");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeProposal_Node_ExistingId_ExistingModelId",
                table: "NodeProposal");

            migrationBuilder.DropTable(
                name: "Node");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternalNode",
                table: "InternalNode");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Element1dId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_RatioAlongElement1d",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes");

            migrationBuilder.RenameTable(
                name: "InternalNode",
                newName: "InternalNodes");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes",
                newName: "Restraint_CanTranslateAlongZ");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes",
                newName: "Restraint_CanTranslateAlongY");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes",
                newName: "Restraint_CanTranslateAlongX");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes",
                newName: "Restraint_CanRotateAboutZ");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes",
                newName: "Restraint_CanRotateAboutY");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes",
                newName: "Restraint_CanRotateAboutX");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_LocationPoint_Z",
                table: "Nodes",
                newName: "LocationPoint_Z");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_LocationPoint_Y",
                table: "Nodes",
                newName: "LocationPoint_Y");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_LocationPoint_X",
                table: "Nodes",
                newName: "LocationPoint_X");

            migrationBuilder.RenameIndex(
                name: "IX_InternalNode_ModelId1",
                table: "InternalNodes",
                newName: "IX_InternalNodes_ModelId1");

            migrationBuilder.RenameIndex(
                name: "IX_InternalNode_ModelId",
                table: "InternalNodes",
                newName: "IX_InternalNodes_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_InternalNode_Element1dId_ModelId",
                table: "InternalNodes",
                newName: "IX_InternalNodes_Element1dId_ModelId");

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternalNodes",
                table: "InternalNodes",
                columns: new[] { "Id", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes",
                column: "ModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNodes_Element1ds_Element1dId_ModelId",
                table: "InternalNodes",
                columns: new[] { "Element1dId", "ModelId" },
                principalTable: "Element1ds",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNodes_Models_ModelId",
                table: "InternalNodes",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNodes_Models_ModelId1",
                table: "InternalNodes",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternalNodes_Element1ds_Element1dId_ModelId",
                table: "InternalNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalNodes_Models_ModelId",
                table: "InternalNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalNodes_Models_ModelId1",
                table: "InternalNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                table: "NodeProposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternalNodes",
                table: "InternalNodes");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "Nodes");

            migrationBuilder.RenameTable(
                name: "InternalNodes",
                newName: "InternalNode");

            migrationBuilder.RenameColumn(
                name: "Restraint_CanTranslateAlongZ",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanTranslateAlongZ");

            migrationBuilder.RenameColumn(
                name: "Restraint_CanTranslateAlongY",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanTranslateAlongY");

            migrationBuilder.RenameColumn(
                name: "Restraint_CanTranslateAlongX",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanTranslateAlongX");

            migrationBuilder.RenameColumn(
                name: "Restraint_CanRotateAboutZ",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanRotateAboutZ");

            migrationBuilder.RenameColumn(
                name: "Restraint_CanRotateAboutY",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanRotateAboutY");

            migrationBuilder.RenameColumn(
                name: "Restraint_CanRotateAboutX",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanRotateAboutX");

            migrationBuilder.RenameColumn(
                name: "LocationPoint_Z",
                table: "Nodes",
                newName: "SpatialNodeDefinition_LocationPoint_Z");

            migrationBuilder.RenameColumn(
                name: "LocationPoint_Y",
                table: "Nodes",
                newName: "SpatialNodeDefinition_LocationPoint_Y");

            migrationBuilder.RenameColumn(
                name: "LocationPoint_X",
                table: "Nodes",
                newName: "SpatialNodeDefinition_LocationPoint_X");

            migrationBuilder.RenameIndex(
                name: "IX_InternalNodes_ModelId1",
                table: "InternalNode",
                newName: "IX_InternalNode_ModelId1");

            migrationBuilder.RenameIndex(
                name: "IX_InternalNodes_ModelId",
                table: "InternalNode",
                newName: "IX_InternalNode_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_InternalNodes_Element1dId_ModelId",
                table: "InternalNode",
                newName: "IX_InternalNode_Element1dId_ModelId");

            migrationBuilder.AddColumn<int>(
                name: "InternalNodeDefinition_Element1dId",
                table: "Nodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "InternalNodeDefinition_RatioAlongElement1d",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "InternalNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InternalNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InternalNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InternalNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InternalNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InternalNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternalNode",
                table: "InternalNode",
                columns: new[] { "Id", "ModelId" });

            migrationBuilder.CreateTable(
                name: "Node",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"NodeDefinitionSequence\"')"),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeDiscriminator = table.Column<string>(type: "text", nullable: false),
                    Element1dId = table.Column<int>(type: "integer", nullable: true),
                    Element1dModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelId1 = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Node", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Node_Element1ds_Element1dId_Element1dModelId",
                        columns: x => new { x.Element1dId, x.Element1dModelId },
                        principalTable: "Element1ds",
                        principalColumns: new[] { "Id", "ModelId" });
                    table.ForeignKey(
                        name: "FK_Node_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Node_Models_ModelId1",
                        column: x => x.ModelId1,
                        principalTable: "Models",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Node_Element1dId_Element1dModelId",
                table: "Node",
                columns: new[] { "Element1dId", "Element1dModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Node_ModelId",
                table: "Node",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Node_ModelId1",
                table: "Node",
                column: "ModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNode_Element1ds_Element1dId_ModelId",
                table: "InternalNode",
                columns: new[] { "Element1dId", "ModelId" },
                principalTable: "Element1ds",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNode_Models_ModelId",
                table: "InternalNode",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalNode_Models_ModelId1",
                table: "InternalNode",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeProposal_Node_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" },
                principalTable: "Node",
                principalColumns: new[] { "Id", "ModelId" });
        }
    }
}
