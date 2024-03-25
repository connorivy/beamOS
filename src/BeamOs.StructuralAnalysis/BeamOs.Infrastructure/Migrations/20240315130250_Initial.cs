using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeamOs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ModulusOfElasticity = table.Column<double>(type: "float", nullable: false),
                        ModulusOfRigidity = table.Column<double>(type: "float", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        Settings_UnitSettings_AngleUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_AreaMomentOfInertiaUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_AreaUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_ForcePerLengthUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_ForceUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_LengthUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_PressureUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_TorqueUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        ),
                        Settings_UnitSettings_VolumeUnit = table.Column<int>(
                            type: "int",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "MomentLoads",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Torque = table.Column<double>(type: "float", nullable: false),
                        NormalizedAxisDirection = table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomentLoads", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "SectionProfiles",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Area = table.Column<double>(type: "float", nullable: false),
                        StrongAxisMomentOfInertia = table.Column<double>(
                            type: "float",
                            nullable: false
                        ),
                        WeakAxisMomentOfInertia = table.Column<double>(
                            type: "float",
                            nullable: false
                        ),
                        PolarMomentOfInertia = table.Column<double>(type: "float", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProfiles", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Element1Ds",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        StartNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        EndNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        SectionProfileId = table.Column<Guid>(
                            type: "uniqueidentifier",
                            nullable: false
                        ),
                        SectionProfileRotation = table.Column<double>(
                            type: "float",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element1Ds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Element1Ds_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        LocationPoint_XCoordinate = table.Column<double>(
                            type: "float",
                            nullable: false
                        ),
                        LocationPoint_YCoordinate = table.Column<double>(
                            type: "float",
                            nullable: false
                        ),
                        LocationPoint_ZCoordinate = table.Column<double>(
                            type: "float",
                            nullable: false
                        ),
                        Restraint_CanRotateAboutX = table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),
                        Restraint_CanRotateAboutY = table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),
                        Restraint_CanRotateAboutZ = table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),
                        Restraint_CanTranslateAlongX = table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),
                        Restraint_CanTranslateAlongY = table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),
                        Restraint_CanTranslateAlongZ = table.Column<bool>(
                            type: "bit",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nodes_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PointLoads",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Force = table.Column<double>(type: "float", nullable: false),
                        NormalizedDirection = table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointLoads_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Element1Ds_ModelId",
                table: "Element1Ds",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModelId",
                table: "Nodes",
                column: "ModelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PointLoads_NodeId",
                table: "PointLoads",
                column: "NodeId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Element1Ds");

            migrationBuilder.DropTable(name: "Materials");

            migrationBuilder.DropTable(name: "MomentLoads");

            migrationBuilder.DropTable(name: "PointLoads");

            migrationBuilder.DropTable(name: "SectionProfiles");

            migrationBuilder.DropTable(name: "Nodes");

            migrationBuilder.DropTable(name: "Models");
        }
    }
}
