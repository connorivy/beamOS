using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionProfileLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "WeakAxisShearArea",
                table: "SectionProfiles",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "StrongAxisShearArea",
                table: "SectionProfiles",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SectionProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "StrongAxisPlasticSectionModulus",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WeakAxisPlasticSectionModulus",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "EnvelopeResultSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvelopeResultSets", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_EnvelopeResultSets_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionProfileFromLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Library = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "EnvelopeElement1dResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Element1dId = table.Column<int>(type: "integer", nullable: false),
                    EnvelopeResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxDisplacement_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxDisplacement_Value = table.Column<double>(type: "double precision", nullable: false),
                    MaxMoment_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxMoment_Value = table.Column<double>(type: "double precision", nullable: false),
                    MaxShear_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxShear_Value = table.Column<double>(type: "double precision", nullable: false),
                    MinDisplacement_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MinDisplacement_Value = table.Column<double>(type: "double precision", nullable: false),
                    MinMoment_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MinMoment_Value = table.Column<double>(type: "double precision", nullable: false),
                    MinShear_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MinShear_Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvelopeElement1dResults", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_EnvelopeElement1dResults_EnvelopeResultSets_EnvelopeResultS~",
                        columns: x => new { x.EnvelopeResultSetId, x.ModelId },
                        principalTable: "EnvelopeResultSets",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvelopeElement1dResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeElement1dResults_EnvelopeResultSetId_ModelId",
                table: "EnvelopeElement1dResults",
                columns: new[] { "EnvelopeResultSetId", "ModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeElement1dResults_ModelId",
                table: "EnvelopeElement1dResults",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeResultSets_ModelId",
                table: "EnvelopeResultSets",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileFromLibrary_ModelId",
                table: "SectionProfileFromLibrary",
                column: "ModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvelopeElement1dResults");

            migrationBuilder.DropTable(
                name: "SectionProfileFromLibrary");

            migrationBuilder.DropTable(
                name: "EnvelopeResultSets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SectionProfiles");

            migrationBuilder.DropColumn(
                name: "StrongAxisPlasticSectionModulus",
                table: "SectionProfiles");

            migrationBuilder.DropColumn(
                name: "WeakAxisPlasticSectionModulus",
                table: "SectionProfiles");

            migrationBuilder.AlterColumn<double>(
                name: "WeakAxisShearArea",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "StrongAxisShearArea",
                table: "SectionProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
