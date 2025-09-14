#if Postgres
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeamOs.StructuralAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    internal partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Settings_YAxisUp = table.Column<bool>(type: "boolean", nullable: false),
                    Settings_AnalysisSettings_Element1DAnalysisType = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_AngleUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_AreaMomentOfInertiaUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_AreaUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_ForcePerLengthUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_ForceUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_LengthUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_PressureUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_TorqueUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_VolumeUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "EnvelopeResultSets",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvelopeResultSets", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_EnvelopeResultSets_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "LoadCases",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadCases", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_LoadCases_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "LoadCombinations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoadCaseFactors = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadCombinations", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_LoadCombinations_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModulusOfElasticity = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    ModulusOfRigidity = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Materials_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ModelProposals",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Settings_YAxisUp = table.Column<bool>(type: "boolean", nullable: false),
                    Settings_AnalysisSettings_Element1DAnalysisType = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_AngleUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_AreaMomentOfInertiaUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_AreaUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_ForcePerLengthUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_ForceUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_LengthUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_PressureUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_TorqueUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    Settings_UnitSettings_VolumeUnit = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelProposals", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_ModelProposals_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "NodeDefinition",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeDefinition", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_NodeDefinition_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "SectionProfileInfoBase",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(
                        type: "character varying(34)",
                        maxLength: 34,
                        nullable: false
                    ),
                    Area = table.Column<double>(type: "double precision", nullable: true),
                    StrongAxisMomentOfInertia = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    WeakAxisMomentOfInertia = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    PolarMomentOfInertia = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    StrongAxisPlasticSectionModulus = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    WeakAxisPlasticSectionModulus = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    StrongAxisShearArea = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    WeakAxisShearArea = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    Library = table.Column<int>(type: "integer", nullable: true),
                    ModelId1 = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfileInfoBase", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_SectionProfileInfoBase_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_SectionProfileInfoBase_Models_ModelId1",
                        column: x => x.ModelId1,
                        principalTable: "Models",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "EnvelopeElement1dResults",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Element1dId = table.Column<int>(type: "integer", nullable: false),
                    EnvelopeResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxDisplacement_ResultSetId = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    MaxDisplacement_Value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    MaxMoment_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxMoment_Value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    MaxShear_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxShear_Value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    MinDisplacement_ResultSetId = table.Column<int>(
                        type: "integer",
                        nullable: false
                    ),
                    MinDisplacement_Value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    MinMoment_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MinMoment_Value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    MinShear_ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MinShear_Value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvelopeElement1dResults", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_EnvelopeElement1dResults_EnvelopeResultSets_EnvelopeResultS~",
                        columns: x => new { x.EnvelopeResultSetId, x.ModelId },
                        principalTable: "EnvelopeResultSets",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_EnvelopeElement1dResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ResultSets",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoadCombinationId = table.Column<int>(type: "integer", nullable: false),
                    LoadCombinationModelId = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultSets", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_ResultSets_LoadCombinations_LoadCombinationId_LoadCombinati~",
                        columns: x => new { x.LoadCombinationId, x.LoadCombinationModelId },
                        principalTable: "LoadCombinations",
                        principalColumns: new[] { "Id", "ModelId" }
                    );
                    table.ForeignKey(
                        name: "FK_ResultSets_LoadCombinations_LoadCombinationId_ModelId",
                        columns: x => new { x.LoadCombinationId, x.ModelId },
                        principalTable: "LoadCombinations",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ResultSets_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "DeleteModelEntityProposal",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    ModelEntityId = table.Column<int>(type: "integer", nullable: false),
                    ObjectType = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_DeleteModelEntityProposal",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_DeleteModelEntityProposal_ModelProposals_ModelProposalId_Mo~",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_DeleteModelEntityProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Element1dProposal",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    EndNodeId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    EndNodeId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    MaterialId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    MaterialId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    SectionProfileId_ExistingId = table.Column<int>(
                        type: "integer",
                        nullable: true
                    ),
                    SectionProfileId_ProposedId = table.Column<int>(
                        type: "integer",
                        nullable: true
                    ),
                    StartNodeId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    StartNodeId_ProposedId = table.Column<int>(type: "integer", nullable: true),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Element1dProposal",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_Element1dProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Element1dProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "InternalNodeProposal",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    RatioAlongElement1d = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Element1dId_ExistingId = table.Column<int>(type: "integer", nullable: true),
                    Element1dId_ProposedId = table.Column<int>(type: "integer", nullable: true),
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
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_InternalNodeProposal",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_InternalNodeProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_InternalNodeProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "MaterialProposal",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    ModulusOfElasticity = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    ModulusOfRigidity = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_MaterialProposal",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_MaterialProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_MaterialProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "NodeProposal",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
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
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_NodeProposal",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_NodeProposal_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_NodeProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProposalIssue",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    ProposedId = table.Column<int>(type: "integer", nullable: true),
                    ObjectType = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ProposalIssue",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_ProposalIssue_ModelProposals_ModelProposalId_ModelId",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ProposalIssue_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "SectionProfileProposal",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    StrongAxisMomentOfInertia = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    WeakAxisMomentOfInertia = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    PolarMomentOfInertia = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    StrongAxisPlasticSectionModulus = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    WeakAxisPlasticSectionModulus = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    StrongAxisShearArea = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    WeakAxisShearArea = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_SectionProfileProposal",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_SectionProfileProposal_ModelProposals_ModelProposalId_Model~",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_SectionProfileProposal_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "SectionProfileProposalFromLibrary",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelProposalId = table.Column<int>(type: "integer", nullable: false),
                    Library = table.Column<int>(type: "integer", nullable: false),
                    ExistingId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_SectionProfileProposalFromLibrary",
                        x => new
                        {
                            x.Id,
                            x.ModelProposalId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_SectionProfileProposalFromLibrary_ModelProposals_ModelPropo~",
                        columns: x => new { x.ModelProposalId, x.ModelId },
                        principalTable: "ModelProposals",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_SectionProfileProposalFromLibrary_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "MomentLoads",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    LoadCaseId = table.Column<int>(type: "integer", nullable: false),
                    Torque = table.Column<double>(type: "double precision", nullable: false),
                    AxisDirection_X = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    AxisDirection_Y = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    AxisDirection_Z = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomentLoads", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_MomentLoads_LoadCases_LoadCaseId_ModelId",
                        columns: x => new { x.LoadCaseId, x.ModelId },
                        principalTable: "LoadCases",
                        principalColumns: new[] { "Id", "ModelId" }
                    );
                    table.ForeignKey(
                        name: "FK_MomentLoads_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_MomentLoads_NodeDefinition_NodeId_ModelId",
                        columns: x => new { x.NodeId, x.ModelId },
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

            migrationBuilder.CreateTable(
                name: "PointLoads",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    LoadCaseId = table.Column<int>(type: "integer", nullable: false),
                    Force = table.Column<double>(type: "double precision", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointLoads", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_PointLoads_LoadCases_LoadCaseId_ModelId",
                        columns: x => new { x.LoadCaseId, x.ModelId },
                        principalTable: "LoadCases",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_PointLoads_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_PointLoads_NodeDefinition_NodeId_ModelId",
                        columns: x => new { x.NodeId, x.ModelId },
                        principalTable: "NodeDefinition",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Element1ds",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartNodeId = table.Column<int>(type: "integer", nullable: false),
                    EndNodeId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    SectionProfileId = table.Column<int>(type: "integer", nullable: false),
                    SectionProfileRotation = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1ds", x => new { x.Id, x.ModelId });
                    table.ForeignKey(
                        name: "FK_Element1ds_Materials_MaterialId_ModelId",
                        columns: x => new { x.MaterialId, x.ModelId },
                        principalTable: "Materials",
                        principalColumns: new[] { "Id", "ModelId" }
                    );
                    table.ForeignKey(
                        name: "FK_Element1ds_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Element1ds_NodeDefinition_EndNodeId_ModelId",
                        columns: x => new { x.EndNodeId, x.ModelId },
                        principalTable: "NodeDefinition",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Element1ds_NodeDefinition_StartNodeId_ModelId",
                        columns: x => new { x.StartNodeId, x.ModelId },
                        principalTable: "NodeDefinition",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Element1ds_SectionProfileInfoBase_SectionProfileId_ModelId",
                        columns: x => new { x.SectionProfileId, x.ModelId },
                        principalTable: "SectionProfileInfoBase",
                        principalColumns: new[] { "Id", "ModelId" }
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Element1dResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    MaxShear = table.Column<double>(type: "double precision", nullable: false),
                    MinShear = table.Column<double>(type: "double precision", nullable: false),
                    MaxMoment = table.Column<double>(type: "double precision", nullable: false),
                    MinMoment = table.Column<double>(type: "double precision", nullable: false),
                    MaxDisplacement = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    MinDisplacement = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Element1dResults",
                        x => new
                        {
                            x.Id,
                            x.ResultSetId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_Element1dResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Element1dResults_ResultSets_ResultSetId_ModelId",
                        columns: x => new { x.ResultSetId, x.ModelId },
                        principalTable: "ResultSets",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "NodeResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultSetId = table.Column<int>(type: "integer", nullable: false),
                    Displacements_DisplacementAlongX = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Displacements_DisplacementAlongY = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Displacements_DisplacementAlongZ = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Displacements_RotationAboutX = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Displacements_RotationAboutY = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Displacements_RotationAboutZ = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Forces_ForceAlongX = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Forces_ForceAlongY = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Forces_ForceAlongZ = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Forces_MomentAboutX = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Forces_MomentAboutY = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Forces_MomentAboutZ = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_NodeResults",
                        x => new
                        {
                            x.Id,
                            x.ResultSetId,
                            x.ModelId,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_NodeResults_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_NodeResults_ResultSets_ResultSetId_ModelId",
                        columns: x => new { x.ResultSetId, x.ModelId },
                        principalTable: "ResultSets",
                        principalColumns: new[] { "Id", "ModelId" },
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "InternalNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    RatioAlongElement1d = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Element1dId = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_DeleteModelEntityProposal_ModelId",
                table: "DeleteModelEntityProposal",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_DeleteModelEntityProposal_ModelProposalId_ModelId",
                table: "DeleteModelEntityProposal",
                columns: new[] { "ModelProposalId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1dProposal_ModelId",
                table: "Element1dProposal",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1dProposal_ModelProposalId_ModelId",
                table: "Element1dProposal",
                columns: new[] { "ModelProposalId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1dResults_ModelId",
                table: "Element1dResults",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1dResults_ResultSetId_ModelId",
                table: "Element1dResults",
                columns: new[] { "ResultSetId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_EndNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "EndNodeId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_MaterialId_ModelId",
                table: "Element1ds",
                columns: new[] { "MaterialId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_ModelId",
                table: "Element1ds",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_SectionProfileId_ModelId",
                table: "Element1ds",
                columns: new[] { "SectionProfileId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1ds_StartNodeId_ModelId",
                table: "Element1ds",
                columns: new[] { "StartNodeId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeElement1dResults_EnvelopeResultSetId_ModelId",
                table: "EnvelopeElement1dResults",
                columns: new[] { "EnvelopeResultSetId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeElement1dResults_ModelId",
                table: "EnvelopeElement1dResults",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeResultSets_ModelId",
                table: "EnvelopeResultSets",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_InternalNodeProposal_ModelId",
                table: "InternalNodeProposal",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_InternalNodeProposal_ModelProposalId_ModelId",
                table: "InternalNodeProposal",
                columns: new[] { "ModelProposalId", "ModelId" }
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
                name: "IX_LoadCases_ModelId",
                table: "LoadCases",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LoadCombinations_ModelId",
                table: "LoadCombinations",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProposal_ModelId",
                table: "MaterialProposal",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProposal_ModelProposalId_ModelId",
                table: "MaterialProposal",
                columns: new[] { "ModelProposalId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ModelId",
                table: "Materials",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ModelProposals_ModelId",
                table: "ModelProposals",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_LoadCaseId_ModelId",
                table: "MomentLoads",
                columns: new[] { "LoadCaseId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_ModelId",
                table: "MomentLoads",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MomentLoads_NodeId_ModelId",
                table: "MomentLoads",
                columns: new[] { "NodeId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_NodeDefinition_ModelId",
                table: "NodeDefinition",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_NodeProposal_ModelId",
                table: "NodeProposal",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_NodeProposal_ModelProposalId_ModelId",
                table: "NodeProposal",
                columns: new[] { "ModelProposalId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_NodeResults_ModelId",
                table: "NodeResults",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_NodeResults_ResultSetId_ModelId",
                table: "NodeResults",
                columns: new[] { "ResultSetId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId",
                table: "Nodes",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_LoadCaseId_ModelId",
                table: "PointLoads",
                columns: new[] { "LoadCaseId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_ModelId",
                table: "PointLoads",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_NodeId_ModelId",
                table: "PointLoads",
                columns: new[] { "NodeId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProposalIssue_ModelId_ModelProposalId_ExistingId_ProposedId",
                table: "ProposalIssue",
                columns: new[] { "ModelId", "ModelProposalId", "ExistingId", "ProposedId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProposalIssue_ModelProposalId_ModelId",
                table: "ProposalIssue",
                columns: new[] { "ModelProposalId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ResultSets_LoadCombinationId_LoadCombinationModelId",
                table: "ResultSets",
                columns: new[] { "LoadCombinationId", "LoadCombinationModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ResultSets_LoadCombinationId_ModelId",
                table: "ResultSets",
                columns: new[] { "LoadCombinationId", "ModelId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ResultSets_ModelId",
                table: "ResultSets",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileInfoBase_ModelId",
                table: "SectionProfileInfoBase",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileInfoBase_ModelId1",
                table: "SectionProfileInfoBase",
                column: "ModelId1"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposal_ModelId",
                table: "SectionProfileProposal",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposal_ModelProposalId_ModelId",
                table: "SectionProfileProposal",
                columns: new[] { "ModelProposalId", "ModelId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposalFromLibrary_ModelId",
                table: "SectionProfileProposalFromLibrary",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SectionProfileProposalFromLibrary_ModelProposalId_ModelId",
                table: "SectionProfileProposalFromLibrary",
                columns: new[] { "ModelProposalId", "ModelId" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DeleteModelEntityProposal");

            migrationBuilder.DropTable(name: "Element1dProposal");

            migrationBuilder.DropTable(name: "Element1dResults");

            migrationBuilder.DropTable(name: "EnvelopeElement1dResults");

            migrationBuilder.DropTable(name: "InternalNodeProposal");

            migrationBuilder.DropTable(name: "InternalNodes");

            migrationBuilder.DropTable(name: "MaterialProposal");

            migrationBuilder.DropTable(name: "MomentLoads");

            migrationBuilder.DropTable(name: "NodeProposal");

            migrationBuilder.DropTable(name: "NodeResults");

            migrationBuilder.DropTable(name: "Nodes");

            migrationBuilder.DropTable(name: "PointLoads");

            migrationBuilder.DropTable(name: "ProposalIssue");

            migrationBuilder.DropTable(name: "SectionProfileProposal");

            migrationBuilder.DropTable(name: "SectionProfileProposalFromLibrary");

            migrationBuilder.DropTable(name: "EnvelopeResultSets");

            migrationBuilder.DropTable(name: "Element1ds");

            migrationBuilder.DropTable(name: "ResultSets");

            migrationBuilder.DropTable(name: "LoadCases");

            migrationBuilder.DropTable(name: "ModelProposals");

            migrationBuilder.DropTable(name: "Materials");

            migrationBuilder.DropTable(name: "NodeDefinition");

            migrationBuilder.DropTable(name: "SectionProfileInfoBase");

            migrationBuilder.DropTable(name: "LoadCombinations");

            migrationBuilder.DropTable(name: "Models");
        }
    }
}

#endif
