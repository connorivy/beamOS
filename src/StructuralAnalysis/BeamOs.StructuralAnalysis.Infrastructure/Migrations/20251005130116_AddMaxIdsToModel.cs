using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxIdsToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.DropColumn(
                name: "ModelEntityId",
                table: "DeleteModelEntityProposals");

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

            migrationBuilder.AddColumn<int>(
                name: "MaxElement1dId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxInternalNodeId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoadCaseId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoadCombinationId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxMaterialId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxModelProposalId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxMomentLoadId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxNodeId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxPointLoadId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSectionProfileFromLibraryId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSectionProfileId",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxElement1dId",
                table: "ModelProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxInternalNodeId",
                table: "ModelProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxMaterialId",
                table: "ModelProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxNodeId",
                table: "ModelProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSectionProfileId",
                table: "ModelProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Materials",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                columns: new[] { "Id", "ObjectType", "ModelProposalId", "ModelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals");

            migrationBuilder.DropColumn(
                name: "MaxElement1dId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxInternalNodeId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxLoadCaseId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxLoadCombinationId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxMaterialId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxModelProposalId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxMomentLoadId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxNodeId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxPointLoadId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxSectionProfileFromLibraryId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxSectionProfileId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "MaxElement1dId",
                table: "ModelProposals");

            migrationBuilder.DropColumn(
                name: "MaxInternalNodeId",
                table: "ModelProposals");

            migrationBuilder.DropColumn(
                name: "MaxMaterialId",
                table: "ModelProposals");

            migrationBuilder.DropColumn(
                name: "MaxNodeId",
                table: "ModelProposals");

            migrationBuilder.DropColumn(
                name: "MaxSectionProfileId",
                table: "ModelProposals");

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

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Materials",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Element1ds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ModelEntityId",
                table: "DeleteModelEntityProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeleteModelEntityProposals",
                table: "DeleteModelEntityProposals",
                columns: new[] { "Id", "ModelProposalId", "ModelId" });
        }
    }
}
