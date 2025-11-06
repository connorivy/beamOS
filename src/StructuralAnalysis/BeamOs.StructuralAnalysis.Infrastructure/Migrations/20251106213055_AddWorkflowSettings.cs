using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SectionProfileInfoBase",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NodeDefinitions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "BimSourceModelId",
                table: "Models",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Settings_WorkflowSettings_BimFirstModelIdsSerialized",
                table: "Models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Settings_WorkflowSettings_ModelingMode",
                table: "Models",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Settings_WorkflowSettings_BimFirstModelIdsSerialized",
                table: "ModelProposals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Settings_WorkflowSettings_ModelingMode",
                table: "ModelProposals",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Element1ds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Element1ds",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_BimSourceModelId",
                table: "Models",
                column: "BimSourceModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Models_Models_BimSourceModelId",
                table: "Models",
                column: "BimSourceModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Models_Models_BimSourceModelId",
                table: "Models");

            migrationBuilder.DropIndex(
                name: "IX_Models_BimSourceModelId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "BimSourceModelId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Settings_WorkflowSettings_BimFirstModelIdsSerialized",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Settings_WorkflowSettings_ModelingMode",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Settings_WorkflowSettings_BimFirstModelIdsSerialized",
                table: "ModelProposals");

            migrationBuilder.DropColumn(
                name: "Settings_WorkflowSettings_ModelingMode",
                table: "ModelProposals");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Element1ds");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SectionProfileInfoBase",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NodeDefinitions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Element1ds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
