using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalNode : Migration
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

            migrationBuilder.DropForeignKey(
                name: "FK_MomentLoads_Nodes_NodeId_ModelId",
                table: "MomentLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                table: "NodeProposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_ModelId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nodes",
                table: "Nodes");

            migrationBuilder.RenameTable(
                name: "Nodes",
                newName: "NodeBase");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_ModelId",
                table: "NodeBase",
                newName: "IX_NodeBase_ModelId");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanTranslateAlongZ",
                table: "NodeBase",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanTranslateAlongY",
                table: "NodeBase",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanTranslateAlongX",
                table: "NodeBase",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanRotateAboutZ",
                table: "NodeBase",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanRotateAboutY",
                table: "NodeBase",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanRotateAboutX",
                table: "NodeBase",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<double>(
                name: "LocationPoint_Z",
                table: "NodeBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "LocationPoint_Y",
                table: "NodeBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "LocationPoint_X",
                table: "NodeBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "NodeBase",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Element1dId",
                table: "NodeBase",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RatioAlongElement1d",
                table: "NodeBase",
                type: "double precision",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "NodeBase");

            migrationBuilder.DropColumn(
                name: "Element1dId",
                table: "NodeBase");

            migrationBuilder.DropColumn(
                name: "RatioAlongElement1d",
                table: "NodeBase");

            migrationBuilder.RenameTable(
                name: "NodeBase",
                newName: "Nodes");

            migrationBuilder.RenameIndex(
                name: "IX_NodeBase_ModelId",
                table: "Nodes",
                newName: "IX_Nodes_ModelId");

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanTranslateAlongZ",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanTranslateAlongY",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanTranslateAlongX",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanRotateAboutZ",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanRotateAboutY",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Restraint_CanRotateAboutX",
                table: "Nodes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LocationPoint_Z",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LocationPoint_Y",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LocationPoint_X",
                table: "Nodes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nodes",
                table: "Nodes",
                columns: new[] { "Id", "ModelId" });

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
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_NodeProposal_Nodes_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" });

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
    }
}
