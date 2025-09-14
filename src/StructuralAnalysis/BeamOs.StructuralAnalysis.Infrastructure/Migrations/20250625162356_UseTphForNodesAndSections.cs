#if Postgres
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    internal partial class UseTphForNodesAndSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_NodeDefinition_EndNodeId_ModelId",
                table: "Element1ds"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_NodeDefinition_StartNodeId_ModelId",
                table: "Element1ds"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_MomentLoads_NodeDefinition_NodeId_ModelId",
                table: "MomentLoads"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_NodeDefinition_Models_ModelId",
                table: "NodeDefinition"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_NodeDefinition_NodeId_ModelId",
                table: "PointLoads"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_SectionProfileInfoBase_Models_ModelId1",
                table: "SectionProfileInfoBase"
            );

            migrationBuilder.DropTable(name: "InternalNodes");

            migrationBuilder.DropTable(name: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_SectionProfileInfoBase_ModelId1",
                table: "SectionProfileInfoBase"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_NodeDefinition", table: "NodeDefinition");

            migrationBuilder.DropColumn(name: "Discriminator", table: "SectionProfileInfoBase");

            migrationBuilder.DropColumn(name: "ModelId1", table: "SectionProfileInfoBase");

            migrationBuilder.RenameTable(name: "NodeDefinition", newName: "NodeDefinitions");

            migrationBuilder.RenameIndex(
                name: "IX_NodeDefinition_ModelId",
                table: "NodeDefinitions",
                newName: "IX_NodeDefinitions_ModelId"
            );

            migrationBuilder.AddColumn<byte>(
                name: "SectionProfileType",
                table: "SectionProfileInfoBase",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AlterColumn<byte>(
                name: "ObjectType",
                table: "ProposalIssue",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AlterColumn<byte>(
                name: "ObjectType",
                table: "DeleteModelEntityProposals",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AddColumn<int>(
                name: "Element1dId",
                table: "NodeDefinitions",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "LocationPoint_X",
                table: "NodeDefinitions",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "LocationPoint_Y",
                table: "NodeDefinitions",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "LocationPoint_Z",
                table: "NodeDefinitions",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<byte>(
                name: "NodeType",
                table: "NodeDefinitions",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AddColumn<double>(
                name: "RatioAlongElement1d",
                table: "NodeDefinitions",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanRotateAboutX",
                table: "NodeDefinitions",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanRotateAboutY",
                table: "NodeDefinitions",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanRotateAboutZ",
                table: "NodeDefinitions",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanTranslateAlongX",
                table: "NodeDefinitions",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanTranslateAlongY",
                table: "NodeDefinitions",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "Restraint_CanTranslateAlongZ",
                table: "NodeDefinitions",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_NodeDefinitions",
                table: "NodeDefinitions",
                columns: new[] { "Id", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_NodeDefinitions_Element1dId_ModelId",
                table: "NodeDefinitions",
                columns: new[] { "Element1dId", "ModelId" }
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_NodeDefinitions_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" },
                principalTable: "NodeDefinitions",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_NodeDefinitions_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" },
                principalTable: "NodeDefinitions",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_MomentLoads_NodeDefinitions_NodeId_ModelId",
                table: "MomentLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "NodeDefinitions",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_NodeDefinitions_Element1ds_Element1dId_ModelId",
                table: "NodeDefinitions",
                columns: new[] { "Element1dId", "ModelId" },
                principalTable: "Element1ds",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_NodeDefinitions_Models_ModelId",
                table: "NodeDefinitions",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_NodeDefinitions_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "NodeDefinitions",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_NodeDefinitions_EndNodeId_ModelId",
                table: "Element1ds"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_NodeDefinitions_StartNodeId_ModelId",
                table: "Element1ds"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_MomentLoads_NodeDefinitions_NodeId_ModelId",
                table: "MomentLoads"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_NodeDefinitions_Element1ds_Element1dId_ModelId",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_NodeDefinitions_Models_ModelId",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PointLoads_NodeDefinitions_NodeId_ModelId",
                table: "PointLoads"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_NodeDefinitions", table: "NodeDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_NodeDefinitions_Element1dId_ModelId",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropColumn(
                name: "SectionProfileType",
                table: "SectionProfileInfoBase"
            );

            migrationBuilder.DropColumn(name: "Element1dId", table: "NodeDefinitions");

            migrationBuilder.DropColumn(name: "LocationPoint_X", table: "NodeDefinitions");

            migrationBuilder.DropColumn(name: "LocationPoint_Y", table: "NodeDefinitions");

            migrationBuilder.DropColumn(name: "LocationPoint_Z", table: "NodeDefinitions");

            migrationBuilder.DropColumn(name: "NodeType", table: "NodeDefinitions");

            migrationBuilder.DropColumn(name: "RatioAlongElement1d", table: "NodeDefinitions");

            migrationBuilder.DropColumn(
                name: "Restraint_CanRotateAboutX",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropColumn(
                name: "Restraint_CanRotateAboutY",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropColumn(
                name: "Restraint_CanRotateAboutZ",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropColumn(
                name: "Restraint_CanTranslateAlongX",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropColumn(
                name: "Restraint_CanTranslateAlongY",
                table: "NodeDefinitions"
            );

            migrationBuilder.DropColumn(
                name: "Restraint_CanTranslateAlongZ",
                table: "NodeDefinitions"
            );

            migrationBuilder.RenameTable(name: "NodeDefinitions", newName: "NodeDefinition");

            migrationBuilder.RenameIndex(
                name: "IX_NodeDefinitions_ModelId",
                table: "NodeDefinition",
                newName: "IX_NodeDefinition_ModelId"
            );

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "SectionProfileInfoBase",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "SectionProfileInfoBase",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "ObjectType",
                table: "ProposalIssue",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint"
            );

            migrationBuilder.AlterColumn<int>(
                name: "ObjectType",
                table: "DeleteModelEntityProposals",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_NodeDefinition",
                table: "NodeDefinition",
                columns: new[] { "Id", "ModelId" }
            );

            migrationBuilder.CreateTable(
                name: "InternalNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Element1dId = table.Column<int>(type: "integer", nullable: false),
                    RatioAlongElement1d = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Restraint_CanRotateAboutX = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanRotateAboutY = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanRotateAboutZ = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanTranslateAlongX = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanTranslateAlongY = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalNodes", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_InternalNodes_Element1ds_Element1dId_ModelId",
                        columns: x => new { x.Element1dId, x.ModelId },
                        principalTable: "Element1ds",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_InternalNodes_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_InternalNodes_NodeDefinition_Id_ModelId",
                        columns: x => new { x.Id, x.ModelId },
                        principalTable: "NodeDefinition",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationPoint_X = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    LocationPoint_Y = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    LocationPoint_Z = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Restraint_CanRotateAboutX = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanRotateAboutY = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanRotateAboutZ = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanTranslateAlongX = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanTranslateAlongY = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                    Restraint_CanTranslateAlongZ = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Nodes_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Nodes_NodeDefinition_Id_ModelId",
                        columns: x => new { x.Id, x.ModelId },
                        principalTable: "NodeDefinition",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileInfoBase_ModelId1",
                table: "SectionProfileInfoBase",
                column: "ModelId1"
            );

            migrationBuilder.CreateIndex(
                name: "IX_InternalNodes_Element1dId_ModelId",
                table: "InternalNodes",
                columns: new[] { "Element1dId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_InternalNodes_ModelId",
                table: "InternalNodes",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId",
                table: "Nodes",
                column: "ModelId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_NodeDefinition_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" },
                principalTable: "NodeDefinition",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_NodeDefinition_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" },
                principalTable: "NodeDefinition",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_MomentLoads_NodeDefinition_NodeId_ModelId",
                table: "MomentLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "NodeDefinition",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_NodeDefinition_Models_ModelId",
                table: "NodeDefinition",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PointLoads_NodeDefinition_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" },
                principalTable: "NodeDefinition",
                principalColumns: new[] { "Id", "ModelId" },
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_SectionProfileInfoBase_Models_ModelId1",
                table: "SectionProfileInfoBase",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id"
            );
        }
    }
}

#endif
