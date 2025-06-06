using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNodeDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_NodeBase_EndNodeId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_NodeBase_StartNodeId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_MomentLoads_NodeBase_NodeId_ModelId",
                table: "MomentLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeBase_Element1ds_Element1dId_ModelId",
                table: "NodeBase");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeBase_Models_ModelId",
                table: "NodeBase");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeProposal_NodeBase_ExistingId_ExistingModelId",
                table: "NodeProposal");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_NodeBase_NodeId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NodeBase",
                table: "NodeBase");

            migrationBuilder.DropIndex(
                name: "IX_NodeBase_Element1dId_ModelId",
                table: "NodeBase");

            migrationBuilder.RenameTable(
                name: "NodeBase",
                newName: "Nodes");

            migrationBuilder.RenameIndex(
                name: "IX_NodeBase_ModelId",
                table: "Nodes",
                newName: "IX_Nodes_ModelId");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Nodes",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<Guid>(
                name: "Element1dModelId",
                table: "Nodes",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "InternalNode_Element1dId",
                table: "Nodes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SpacialNodeDefinition_LocationPoint_X",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpacialNodeDefinition_LocationPoint_Y",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpacialNodeDefinition_LocationPoint_Z",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TypeDiscriminator",
                table: "Nodes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nodes",
                table: "Nodes",
                columns: new[] { "Id", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Element1dId_Element1dModelId",
                table: "Nodes",
                columns: new[] { "Element1dId", "Element1dModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_InternalNode_Element1dId_ModelId",
                table: "Nodes",
                columns: new[] { "InternalNode_Element1dId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MomentLoads_Nodes_NodeId_ModelId",
                table: "MomentLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Element1ds_Element1dId_Element1dModelId",
                table: "Nodes",
                columns: new[] { "Element1dId", "Element1dModelId" },
                principalTable: "Element1ds",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Element1ds_InternalNode_Element1dId_ModelId",
                table: "Nodes",
                columns: new[] { "InternalNode_Element1dId", "ModelId" },
                principalTable: "Element1ds",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Models_ModelId",
                table: "Nodes",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_EndNodeId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_Nodes_StartNodeId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_MomentLoads_Nodes_NodeId_ModelId",
                table: "MomentLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                table: "NodeProposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Element1ds_Element1dId_Element1dModelId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Element1ds_InternalNode_Element1dId_ModelId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_ModelId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nodes",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_Element1dId_Element1dModelId",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_InternalNode_Element1dId_ModelId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Element1dModelId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_Element1dId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNodeDefinition_RatioAlongElement1d",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNode_Element1dId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_LocationPoint_X",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_LocationPoint_Y",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_LocationPoint_Z",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "TypeDiscriminator",
                table: "Nodes");

            migrationBuilder.RenameTable(
                name: "Nodes",
                newName: "NodeBase");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_ModelId",
                table: "NodeBase",
                newName: "IX_NodeBase_ModelId");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "NodeBase",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(21)",
                oldMaxLength: 21);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NodeBase",
                table: "NodeBase",
                columns: new[] { "Id", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_NodeBase_Element1dId_ModelId",
                table: "NodeBase",
                columns: new[] { "Element1dId", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_NodeBase_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" },
                principalTable: "NodeBase",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_NodeBase_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" },
                principalTable: "NodeBase",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MomentLoads_NodeBase_NodeId_ModelId",
                table: "MomentLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "NodeBase",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeBase_Element1ds_Element1dId_ModelId",
                table: "NodeBase",
                columns: new[] { "Element1dId", "ModelId" },
                principalTable: "Element1ds",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeBase_Models_ModelId",
                table: "NodeBase",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeProposal_NodeBase_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" },
                principalTable: "NodeBase",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_NodeBase_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "NodeBase",
                principalColumns: new[] { "Id", "ModelId" });
        }
    }
}
