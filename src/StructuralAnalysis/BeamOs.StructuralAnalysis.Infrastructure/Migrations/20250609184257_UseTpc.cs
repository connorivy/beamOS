using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseTpc : Migration
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
                name: "FK_Nodes_Element1ds_Element1dId_Element1dModelId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Element1ds_InternalNode_Element1dId_ModelId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_InternalNode_ModelId1",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Models_ModelId1",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_Element1dId_Element1dModelId",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_InternalNode_Element1dId_ModelId",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_InternalNode_ModelId1",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Element1dId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Element1dModelId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNode_Element1dId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "InternalNode_ModelId1",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LocationPoint_X",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LocationPoint_Y",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LocationPoint_Z",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "RatioAlongElement1d",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Restraint_CanRotateAboutX",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Restraint_CanRotateAboutY",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Restraint_CanRotateAboutZ",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Restraint_CanTranslateAlongX",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Restraint_CanTranslateAlongY",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Restraint_CanTranslateAlongZ",
                table: "Nodes");

            migrationBuilder.CreateSequence(
                name: "NodeDefinitionSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Nodes",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('\"NodeDefinitionSequence\"')",
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "InternalNode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"NodeDefinitionSequence\"')"),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeDiscriminator = table.Column<string>(type: "text", nullable: false),
                    RatioAlongElement1d = table.Column<double>(type: "double precision", nullable: false),
                    Element1dId = table.Column<int>(type: "integer", nullable: false),
                    ModelId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    Restraint_CanRotateAboutX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutZ = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalNode", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_InternalNode_Element1ds_Element1dId_ModelId",
                        columns: x => new { x.Element1dId, x.ModelId },
                        principalTable: "Element1ds",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InternalNode_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InternalNode_Models_ModelId1",
                        column: x => x.ModelId1,
                        principalTable: "Models",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Node",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"NodeDefinitionSequence\"')"),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeDiscriminator = table.Column<string>(type: "text", nullable: false),
                    ModelId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationPoint_X = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Y = table.Column<double>(type: "double precision", nullable: false),
                    LocationPoint_Z = table.Column<double>(type: "double precision", nullable: false),
                    Restraint_CanRotateAboutX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanRotateAboutZ = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongX = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongY = table.Column<bool>(type: "boolean", nullable: false),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(type: "boolean", nullable: false),
                    Element1dId = table.Column<int>(type: "integer", nullable: true),
                    Element1dModelId = table.Column<Guid>(type: "uuid", nullable: true)
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
                name: "IX_InternalNode_Element1dId_ModelId",
                table: "InternalNode",
                columns: new[] { "Element1dId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_InternalNode_ModelId",
                table: "InternalNode",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalNode_ModelId1",
                table: "InternalNode",
                column: "ModelId1");

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
                name: "FK_NodeProposal_Node_ExistingId_ExistingModelId",
                table: "NodeProposal",
                columns: new[] { "ExistingId", "ExistingModelId" },
                principalTable: "Node",
                principalColumns: new[] { "Id", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeProposal_Node_ExistingId_ExistingModelId",
                table: "NodeProposal");

            migrationBuilder.DropTable(
                name: "InternalNode");

            migrationBuilder.DropTable(
                name: "Node");

            migrationBuilder.DropSequence(
                name: "NodeDefinitionSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Nodes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "nextval('\"NodeDefinitionSequence\"')")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Nodes",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Element1dId",
                table: "Nodes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Element1dModelId",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InternalNode_Element1dId",
                table: "Nodes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InternalNode_ModelId1",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationPoint_X",
                table: "Nodes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationPoint_Y",
                table: "Nodes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationPoint_Z",
                table: "Nodes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RatioAlongElement1d",
                table: "Nodes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanRotateAboutX",
                table: "Nodes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanRotateAboutY",
                table: "Nodes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanRotateAboutZ",
                table: "Nodes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanTranslateAlongX",
                table: "Nodes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanTranslateAlongY",
                table: "Nodes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanTranslateAlongZ",
                table: "Nodes",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Element1dId_Element1dModelId",
                table: "Nodes",
                columns: new[] { "Element1dId", "Element1dModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_InternalNode_Element1dId_ModelId",
                table: "Nodes",
                columns: new[] { "InternalNode_Element1dId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_InternalNode_ModelId1",
                table: "Nodes",
                column: "InternalNode_ModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId1",
                table: "Nodes",
                column: "ModelId1");

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
                name: "FK_PointLoads_Nodes_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "Nodes",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
