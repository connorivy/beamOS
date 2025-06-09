using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalRestraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
