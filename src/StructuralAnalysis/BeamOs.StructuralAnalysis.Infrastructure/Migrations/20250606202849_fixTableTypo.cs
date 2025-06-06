using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixTableTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanTranslateAlongZ");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanTranslateAlongY");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanTranslateAlongX");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanRotateAboutZ");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanRotateAboutY");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes",
                newName: "SpatialNodeDefinition_Restraint_CanRotateAboutX");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_LocationPoint_Z",
                table: "Nodes",
                newName: "SpatialNodeDefinition_LocationPoint_Z");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_LocationPoint_Y",
                table: "Nodes",
                newName: "SpatialNodeDefinition_LocationPoint_Y");

            migrationBuilder.RenameColumn(
                name: "SpacialNodeDefinition_LocationPoint_X",
                table: "Nodes",
                newName: "SpatialNodeDefinition_LocationPoint_X");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanTranslateAlongZ",
                table: "Nodes",
                newName: "SpacialNodeDefinition_Restraint_CanTranslateAlongZ");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanTranslateAlongY",
                table: "Nodes",
                newName: "SpacialNodeDefinition_Restraint_CanTranslateAlongY");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanTranslateAlongX",
                table: "Nodes",
                newName: "SpacialNodeDefinition_Restraint_CanTranslateAlongX");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanRotateAboutZ",
                table: "Nodes",
                newName: "SpacialNodeDefinition_Restraint_CanRotateAboutZ");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanRotateAboutY",
                table: "Nodes",
                newName: "SpacialNodeDefinition_Restraint_CanRotateAboutY");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_Restraint_CanRotateAboutX",
                table: "Nodes",
                newName: "SpacialNodeDefinition_Restraint_CanRotateAboutX");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_LocationPoint_Z",
                table: "Nodes",
                newName: "SpacialNodeDefinition_LocationPoint_Z");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_LocationPoint_Y",
                table: "Nodes",
                newName: "SpacialNodeDefinition_LocationPoint_Y");

            migrationBuilder.RenameColumn(
                name: "SpatialNodeDefinition_LocationPoint_X",
                table: "Nodes",
                newName: "SpacialNodeDefinition_LocationPoint_X");
        }
    }
}
