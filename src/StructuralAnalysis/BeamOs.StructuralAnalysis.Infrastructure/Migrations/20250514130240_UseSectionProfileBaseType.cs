using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseSectionProfileBaseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_SectionProfiles_SectionProfileId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionProfiles_Models_ModelId",
                table: "SectionProfiles");

            migrationBuilder.DropTable(
                name: "SectionProfileFromLibrary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectionProfiles",
                table: "SectionProfiles");

            migrationBuilder.RenameTable(
                name: "SectionProfiles",
                newName: "SectionProfileInfoBase");

            migrationBuilder.RenameIndex(
                name: "IX_SectionProfiles_ModelId",
                table: "SectionProfileInfoBase",
                newName: "IX_SectionProfileInfoBase_ModelId");

            migrationBuilder.AlterColumn<double>(
                name: "WeakAxisPlasticSectionModulus",
                table: "SectionProfileInfoBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "WeakAxisMomentOfInertia",
                table: "SectionProfileInfoBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "StrongAxisPlasticSectionModulus",
                table: "SectionProfileInfoBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "StrongAxisMomentOfInertia",
                table: "SectionProfileInfoBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "PolarMomentOfInertia",
                table: "SectionProfileInfoBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "Area",
                table: "SectionProfileInfoBase",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "SectionProfileInfoBase",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Library",
                table: "SectionProfileInfoBase",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "SectionProfileInfoBase",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectionProfileInfoBase",
                table: "SectionProfileInfoBase",
                columns: new[] { "Id", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileInfoBase_ModelId1",
                table: "SectionProfileInfoBase",
                column: "ModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_SectionProfileInfoBase_SectionProfileId_ModelId",
                table: "Element1ds",
                columns: new[] { "SectionProfileId", "ModelId" },
                principalTable: "SectionProfileInfoBase",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SectionProfileInfoBase_Models_ModelId",
                table: "SectionProfileInfoBase",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionProfileInfoBase_Models_ModelId1",
                table: "SectionProfileInfoBase",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element1ds_SectionProfileInfoBase_SectionProfileId_ModelId",
                table: "Element1ds");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionProfileInfoBase_Models_ModelId",
                table: "SectionProfileInfoBase");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionProfileInfoBase_Models_ModelId1",
                table: "SectionProfileInfoBase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectionProfileInfoBase",
                table: "SectionProfileInfoBase");

            migrationBuilder.DropIndex(
                name: "IX_SectionProfileInfoBase_ModelId1",
                table: "SectionProfileInfoBase");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "SectionProfileInfoBase");

            migrationBuilder.DropColumn(
                name: "Library",
                table: "SectionProfileInfoBase");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "SectionProfileInfoBase");

            migrationBuilder.RenameTable(
                name: "SectionProfileInfoBase",
                newName: "SectionProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_SectionProfileInfoBase_ModelId",
                table: "SectionProfiles",
                newName: "IX_SectionProfiles_ModelId");

            migrationBuilder.AlterColumn<double>(
                name: "WeakAxisPlasticSectionModulus",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "WeakAxisMomentOfInertia",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "StrongAxisPlasticSectionModulus",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "StrongAxisMomentOfInertia",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PolarMomentOfInertia",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Area",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectionProfiles",
                table: "SectionProfiles",
                columns: new[] { "Id", "ModelId" });

            migrationBuilder.CreateTable(
                name: "SectionProfileFromLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Library = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfileFromLibrary", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_SectionProfileFromLibrary_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileFromLibrary_ModelId",
                table: "SectionProfileFromLibrary",
                column: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Element1ds_SectionProfiles_SectionProfileId_ModelId",
                table: "Element1ds",
                columns: new[] { "SectionProfileId", "ModelId" },
                principalTable: "SectionProfiles",
                principalColumns: new[] { "Id", "ModelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SectionProfiles_Models_ModelId",
                table: "SectionProfiles",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
