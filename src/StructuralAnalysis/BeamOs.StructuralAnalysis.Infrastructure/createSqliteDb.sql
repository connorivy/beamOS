CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Models" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Models" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "LastModified" TEXT NOT NULL,
    "MaxNodeId" INTEGER NOT NULL,
    "MaxInternalNodeId" INTEGER NOT NULL,
    "MaxElement1dId" INTEGER NOT NULL,
    "MaxMaterialId" INTEGER NOT NULL,
    "MaxSectionProfileId" INTEGER NOT NULL,
    "MaxSectionProfileFromLibraryId" INTEGER NOT NULL,
    "MaxPointLoadId" INTEGER NOT NULL,
    "MaxMomentLoadId" INTEGER NOT NULL,
    "MaxLoadCaseId" INTEGER NOT NULL,
    "MaxLoadCombinationId" INTEGER NOT NULL,
    "MaxModelProposalId" INTEGER NOT NULL,
    "Settings_YAxisUp" INTEGER NOT NULL,
    "Settings_AnalysisSettings_Element1DAnalysisType" INTEGER NOT NULL,
    "Settings_UnitSettings_AngleUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_AreaMomentOfInertiaUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_AreaUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_ForcePerLengthUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_ForceUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_LengthUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_PressureUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_TorqueUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_VolumeUnit" INTEGER NOT NULL
);

CREATE TABLE "EnvelopeResultSets" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    CONSTRAINT "PK_EnvelopeResultSets" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_EnvelopeResultSets_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "LoadCases" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "PK_LoadCases" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_LoadCases_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "LoadCombinations" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "LoadCaseFactors" TEXT NOT NULL,
    CONSTRAINT "PK_LoadCombinations" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_LoadCombinations_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Materials" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModulusOfElasticity" REAL NOT NULL,
    "ModulusOfRigidity" REAL NOT NULL,
    CONSTRAINT "PK_Materials" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_Materials_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ModelProposals" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "LastModified" TEXT NOT NULL,
    "MaxNodeId" INTEGER NOT NULL,
    "MaxInternalNodeId" INTEGER NOT NULL,
    "MaxElement1dId" INTEGER NOT NULL,
    "MaxMaterialId" INTEGER NOT NULL,
    "MaxSectionProfileId" INTEGER NOT NULL,
    "Settings_YAxisUp" INTEGER NOT NULL,
    "Settings_AnalysisSettings_Element1DAnalysisType" INTEGER NOT NULL,
    "Settings_UnitSettings_AngleUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_AreaMomentOfInertiaUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_AreaUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_ForcePerLengthUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_ForceUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_LengthUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_PressureUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_TorqueUnit" INTEGER NOT NULL,
    "Settings_UnitSettings_VolumeUnit" INTEGER NOT NULL,
    CONSTRAINT "PK_ModelProposals" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_ModelProposals_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SectionProfileInfoBase" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "SectionProfileType" INTEGER NOT NULL,
    "Area" REAL NULL,
    "StrongAxisMomentOfInertia" REAL NULL,
    "WeakAxisMomentOfInertia" REAL NULL,
    "PolarMomentOfInertia" REAL NULL,
    "StrongAxisPlasticSectionModulus" REAL NULL,
    "WeakAxisPlasticSectionModulus" REAL NULL,
    "StrongAxisShearArea" REAL NULL,
    "WeakAxisShearArea" REAL NULL,
    "Library" INTEGER NULL,
    CONSTRAINT "PK_SectionProfileInfoBase" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_SectionProfileInfoBase_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "EnvelopeElement1dResults" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "Element1dId" INTEGER NOT NULL,
    "EnvelopeResultSetId" INTEGER NOT NULL,
    "MaxDisplacement_ResultSetId" INTEGER NOT NULL,
    "MaxDisplacement_Value" REAL NOT NULL,
    "MaxMoment_ResultSetId" INTEGER NOT NULL,
    "MaxMoment_Value" REAL NOT NULL,
    "MaxShear_ResultSetId" INTEGER NOT NULL,
    "MaxShear_Value" REAL NOT NULL,
    "MinDisplacement_ResultSetId" INTEGER NOT NULL,
    "MinDisplacement_Value" REAL NOT NULL,
    "MinMoment_ResultSetId" INTEGER NOT NULL,
    "MinMoment_Value" REAL NOT NULL,
    "MinShear_ResultSetId" INTEGER NOT NULL,
    "MinShear_Value" REAL NOT NULL,
    CONSTRAINT "PK_EnvelopeElement1dResults" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_EnvelopeElement1dResults_EnvelopeResultSets_EnvelopeResultSetId_ModelId" FOREIGN KEY ("EnvelopeResultSetId", "ModelId") REFERENCES "EnvelopeResultSets" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_EnvelopeElement1dResults_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ResultSets" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "LoadCombinationId" INTEGER NOT NULL,
    "LoadCombinationModelId" TEXT NULL,
    CONSTRAINT "PK_ResultSets" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_ResultSets_LoadCombinations_LoadCombinationId_LoadCombinationModelId" FOREIGN KEY ("LoadCombinationId", "LoadCombinationModelId") REFERENCES "LoadCombinations" ("Id", "ModelId"),
    CONSTRAINT "FK_ResultSets_LoadCombinations_LoadCombinationId_ModelId" FOREIGN KEY ("LoadCombinationId", "ModelId") REFERENCES "LoadCombinations" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_ResultSets_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "DeleteModelEntityProposals" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ObjectType" INTEGER NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    CONSTRAINT "PK_DeleteModelEntityProposals" PRIMARY KEY ("Id", "ObjectType", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_DeleteModelEntityProposals_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_DeleteModelEntityProposals_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Element1dProposals" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "EndNodeId_ExistingId" INTEGER NULL,
    "EndNodeId_ProposedId" INTEGER NULL,
    "MaterialId_ExistingId" INTEGER NULL,
    "MaterialId_ProposedId" INTEGER NULL,
    "SectionProfileId_ExistingId" INTEGER NULL,
    "SectionProfileId_ProposedId" INTEGER NULL,
    "StartNodeId_ExistingId" INTEGER NULL,
    "StartNodeId_ProposedId" INTEGER NULL,
    "ExistingId" INTEGER NULL,
    CONSTRAINT "PK_Element1dProposals" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_Element1dProposals_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_Element1dProposals_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "InternalNodeProposal" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "RatioAlongElement1d" REAL NOT NULL,
    "Element1dId_ExistingId" INTEGER NULL,
    "Element1dId_ProposedId" INTEGER NULL,
    "Restraint_CanRotateAboutX" INTEGER NOT NULL,
    "Restraint_CanRotateAboutY" INTEGER NOT NULL,
    "Restraint_CanRotateAboutZ" INTEGER NOT NULL,
    "Restraint_CanTranslateAlongX" INTEGER NOT NULL,
    "Restraint_CanTranslateAlongY" INTEGER NOT NULL,
    "Restraint_CanTranslateAlongZ" INTEGER NOT NULL,
    "ExistingId" INTEGER NULL,
    CONSTRAINT "PK_InternalNodeProposal" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_InternalNodeProposal_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_InternalNodeProposal_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MaterialProposal" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "ModulusOfElasticity" REAL NOT NULL,
    "ModulusOfRigidity" REAL NOT NULL,
    "ExistingId" INTEGER NULL,
    CONSTRAINT "PK_MaterialProposal" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_MaterialProposal_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_MaterialProposal_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "NodeProposal" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "LocationPoint_X" REAL NOT NULL,
    "LocationPoint_Y" REAL NOT NULL,
    "LocationPoint_Z" REAL NOT NULL,
    "Restraint_CanRotateAboutX" INTEGER NOT NULL,
    "Restraint_CanRotateAboutY" INTEGER NOT NULL,
    "Restraint_CanRotateAboutZ" INTEGER NOT NULL,
    "Restraint_CanTranslateAlongX" INTEGER NOT NULL,
    "Restraint_CanTranslateAlongY" INTEGER NOT NULL,
    "Restraint_CanTranslateAlongZ" INTEGER NOT NULL,
    "ExistingId" INTEGER NULL,
    CONSTRAINT "PK_NodeProposal" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_NodeProposal_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_NodeProposal_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProposalIssue" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "ExistingId" INTEGER NULL,
    "ProposedId" INTEGER NULL,
    "ObjectType" INTEGER NOT NULL,
    "Message" TEXT NOT NULL,
    "Severity" INTEGER NOT NULL,
    "Code" INTEGER NOT NULL,
    CONSTRAINT "PK_ProposalIssue" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_ProposalIssue_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_ProposalIssue_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SectionProfileProposal" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "Area" REAL NOT NULL,
    "StrongAxisMomentOfInertia" REAL NOT NULL,
    "WeakAxisMomentOfInertia" REAL NOT NULL,
    "PolarMomentOfInertia" REAL NOT NULL,
    "StrongAxisPlasticSectionModulus" REAL NOT NULL,
    "WeakAxisPlasticSectionModulus" REAL NOT NULL,
    "StrongAxisShearArea" REAL NULL,
    "WeakAxisShearArea" REAL NULL,
    "ExistingId" INTEGER NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "PK_SectionProfileProposal" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_SectionProfileProposal_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_SectionProfileProposal_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SectionProfileProposalFromLibrary" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ModelProposalId" INTEGER NOT NULL,
    "Library" INTEGER NOT NULL,
    "ExistingId" INTEGER NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "PK_SectionProfileProposalFromLibrary" PRIMARY KEY ("Id", "ModelProposalId", "ModelId"),
    CONSTRAINT "FK_SectionProfileProposalFromLibrary_ModelProposals_ModelProposalId_ModelId" FOREIGN KEY ("ModelProposalId", "ModelId") REFERENCES "ModelProposals" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_SectionProfileProposalFromLibrary_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Element1dResults" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ResultSetId" INTEGER NOT NULL,
    "MaxShear" REAL NOT NULL,
    "MinShear" REAL NOT NULL,
    "MaxMoment" REAL NOT NULL,
    "MinMoment" REAL NOT NULL,
    "MaxDisplacement" REAL NOT NULL,
    "MinDisplacement" REAL NOT NULL,
    CONSTRAINT "PK_Element1dResults" PRIMARY KEY ("Id", "ResultSetId", "ModelId"),
    CONSTRAINT "FK_Element1dResults_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Element1dResults_ResultSets_ResultSetId_ModelId" FOREIGN KEY ("ResultSetId", "ModelId") REFERENCES "ResultSets" ("Id", "ModelId") ON DELETE CASCADE
);

CREATE TABLE "NodeResults" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "ResultSetId" INTEGER NOT NULL,
    "Displacements_DisplacementAlongX" REAL NOT NULL,
    "Displacements_DisplacementAlongY" REAL NOT NULL,
    "Displacements_DisplacementAlongZ" REAL NOT NULL,
    "Displacements_RotationAboutX" REAL NOT NULL,
    "Displacements_RotationAboutY" REAL NOT NULL,
    "Displacements_RotationAboutZ" REAL NOT NULL,
    "Forces_ForceAlongX" REAL NOT NULL,
    "Forces_ForceAlongY" REAL NOT NULL,
    "Forces_ForceAlongZ" REAL NOT NULL,
    "Forces_MomentAboutX" REAL NOT NULL,
    "Forces_MomentAboutY" REAL NOT NULL,
    "Forces_MomentAboutZ" REAL NOT NULL,
    CONSTRAINT "PK_NodeResults" PRIMARY KEY ("Id", "ResultSetId", "ModelId"),
    CONSTRAINT "FK_NodeResults_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_NodeResults_ResultSets_ResultSetId_ModelId" FOREIGN KEY ("ResultSetId", "ModelId") REFERENCES "ResultSets" ("Id", "ModelId") ON DELETE CASCADE
);

CREATE TABLE "Element1ds" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "StartNodeId" INTEGER NOT NULL,
    "EndNodeId" INTEGER NOT NULL,
    "MaterialId" INTEGER NOT NULL,
    "SectionProfileId" INTEGER NOT NULL,
    "SectionProfileRotation" REAL NOT NULL,
    CONSTRAINT "PK_Element1ds" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_Element1ds_Materials_MaterialId_ModelId" FOREIGN KEY ("MaterialId", "ModelId") REFERENCES "Materials" ("Id", "ModelId"),
    CONSTRAINT "FK_Element1ds_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Element1ds_SectionProfileInfoBase_SectionProfileId_ModelId" FOREIGN KEY ("SectionProfileId", "ModelId") REFERENCES "SectionProfileInfoBase" ("Id", "ModelId"),
    CONSTRAINT "FK_Element1ds_NodeDefinitions_EndNodeId_ModelId" FOREIGN KEY ("EndNodeId", "ModelId") REFERENCES "NodeDefinitions" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_Element1ds_NodeDefinitions_StartNodeId_ModelId" FOREIGN KEY ("StartNodeId", "ModelId") REFERENCES "NodeDefinitions" ("Id", "ModelId") ON DELETE CASCADE
);

CREATE TABLE "NodeDefinitions" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "NodeType" INTEGER NOT NULL,
    "RatioAlongElement1d" REAL NULL,
    "Element1dId" INTEGER NULL,
    "Restraint_CanRotateAboutX" INTEGER NULL,
    "Restraint_CanRotateAboutY" INTEGER NULL,
    "Restraint_CanRotateAboutZ" INTEGER NULL,
    "Restraint_CanTranslateAlongX" INTEGER NULL,
    "Restraint_CanTranslateAlongY" INTEGER NULL,
    "Restraint_CanTranslateAlongZ" INTEGER NULL,
    "LocationPoint_X" REAL NULL,
    "LocationPoint_Y" REAL NULL,
    "LocationPoint_Z" REAL NULL,
    CONSTRAINT "PK_NodeDefinitions" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_NodeDefinitions_Element1ds_Element1dId_ModelId" FOREIGN KEY ("Element1dId", "ModelId") REFERENCES "Element1ds" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_NodeDefinitions_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MomentLoads" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "NodeId" INTEGER NOT NULL,
    "LoadCaseId" INTEGER NOT NULL,
    "Torque" REAL NOT NULL,
    "AxisDirection_X" REAL NOT NULL,
    "AxisDirection_Y" REAL NOT NULL,
    "AxisDirection_Z" REAL NOT NULL,
    CONSTRAINT "PK_MomentLoads" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_MomentLoads_LoadCases_LoadCaseId_ModelId" FOREIGN KEY ("LoadCaseId", "ModelId") REFERENCES "LoadCases" ("Id", "ModelId"),
    CONSTRAINT "FK_MomentLoads_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MomentLoads_NodeDefinitions_NodeId_ModelId" FOREIGN KEY ("NodeId", "ModelId") REFERENCES "NodeDefinitions" ("Id", "ModelId") ON DELETE CASCADE
);

CREATE TABLE "PointLoads" (
    "Id" INTEGER NOT NULL,
    "ModelId" TEXT NOT NULL,
    "NodeId" INTEGER NOT NULL,
    "LoadCaseId" INTEGER NOT NULL,
    "Force" REAL NOT NULL,
    "Direction" TEXT NOT NULL,
    CONSTRAINT "PK_PointLoads" PRIMARY KEY ("Id", "ModelId"),
    CONSTRAINT "FK_PointLoads_LoadCases_LoadCaseId_ModelId" FOREIGN KEY ("LoadCaseId", "ModelId") REFERENCES "LoadCases" ("Id", "ModelId") ON DELETE CASCADE,
    CONSTRAINT "FK_PointLoads_Models_ModelId" FOREIGN KEY ("ModelId") REFERENCES "Models" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PointLoads_NodeDefinitions_NodeId_ModelId" FOREIGN KEY ("NodeId", "ModelId") REFERENCES "NodeDefinitions" ("Id", "ModelId") ON DELETE CASCADE
);

CREATE INDEX "IX_DeleteModelEntityProposals_ModelId" ON "DeleteModelEntityProposals" ("ModelId");

CREATE INDEX "IX_DeleteModelEntityProposals_ModelProposalId_ModelId" ON "DeleteModelEntityProposals" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_Element1dProposals_ModelId" ON "Element1dProposals" ("ModelId");

CREATE INDEX "IX_Element1dProposals_ModelProposalId_ModelId" ON "Element1dProposals" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_Element1dResults_ModelId" ON "Element1dResults" ("ModelId");

CREATE INDEX "IX_Element1dResults_ResultSetId_ModelId" ON "Element1dResults" ("ResultSetId", "ModelId");

CREATE INDEX "IX_Element1ds_EndNodeId_ModelId" ON "Element1ds" ("EndNodeId", "ModelId");

CREATE INDEX "IX_Element1ds_MaterialId_ModelId" ON "Element1ds" ("MaterialId", "ModelId");

CREATE INDEX "IX_Element1ds_ModelId" ON "Element1ds" ("ModelId");

CREATE INDEX "IX_Element1ds_SectionProfileId_ModelId" ON "Element1ds" ("SectionProfileId", "ModelId");

CREATE INDEX "IX_Element1ds_StartNodeId_ModelId" ON "Element1ds" ("StartNodeId", "ModelId");

CREATE INDEX "IX_EnvelopeElement1dResults_EnvelopeResultSetId_ModelId" ON "EnvelopeElement1dResults" ("EnvelopeResultSetId", "ModelId");

CREATE INDEX "IX_EnvelopeElement1dResults_ModelId" ON "EnvelopeElement1dResults" ("ModelId");

CREATE INDEX "IX_EnvelopeResultSets_ModelId" ON "EnvelopeResultSets" ("ModelId");

CREATE INDEX "IX_InternalNodeProposal_ModelId" ON "InternalNodeProposal" ("ModelId");

CREATE INDEX "IX_InternalNodeProposal_ModelProposalId_ModelId" ON "InternalNodeProposal" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_LoadCases_ModelId" ON "LoadCases" ("ModelId");

CREATE INDEX "IX_LoadCombinations_ModelId" ON "LoadCombinations" ("ModelId");

CREATE INDEX "IX_MaterialProposal_ModelId" ON "MaterialProposal" ("ModelId");

CREATE INDEX "IX_MaterialProposal_ModelProposalId_ModelId" ON "MaterialProposal" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_Materials_ModelId" ON "Materials" ("ModelId");

CREATE INDEX "IX_ModelProposals_ModelId" ON "ModelProposals" ("ModelId");

CREATE INDEX "IX_MomentLoads_LoadCaseId_ModelId" ON "MomentLoads" ("LoadCaseId", "ModelId");

CREATE INDEX "IX_MomentLoads_ModelId" ON "MomentLoads" ("ModelId");

CREATE INDEX "IX_MomentLoads_NodeId_ModelId" ON "MomentLoads" ("NodeId", "ModelId");

CREATE INDEX "IX_NodeDefinitions_Element1dId_ModelId" ON "NodeDefinitions" ("Element1dId", "ModelId");

CREATE INDEX "IX_NodeDefinitions_ModelId" ON "NodeDefinitions" ("ModelId");

CREATE INDEX "IX_NodeProposal_ModelId" ON "NodeProposal" ("ModelId");

CREATE INDEX "IX_NodeProposal_ModelProposalId_ModelId" ON "NodeProposal" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_NodeResults_ModelId" ON "NodeResults" ("ModelId");

CREATE INDEX "IX_NodeResults_ResultSetId_ModelId" ON "NodeResults" ("ResultSetId", "ModelId");

CREATE INDEX "IX_PointLoads_LoadCaseId_ModelId" ON "PointLoads" ("LoadCaseId", "ModelId");

CREATE INDEX "IX_PointLoads_ModelId" ON "PointLoads" ("ModelId");

CREATE INDEX "IX_PointLoads_NodeId_ModelId" ON "PointLoads" ("NodeId", "ModelId");

CREATE UNIQUE INDEX "IX_ProposalIssue_ModelId_ModelProposalId_ExistingId_ProposedId" ON "ProposalIssue" ("ModelId", "ModelProposalId", "ExistingId", "ProposedId");

CREATE INDEX "IX_ProposalIssue_ModelProposalId_ModelId" ON "ProposalIssue" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_ResultSets_LoadCombinationId_LoadCombinationModelId" ON "ResultSets" ("LoadCombinationId", "LoadCombinationModelId");

CREATE UNIQUE INDEX "IX_ResultSets_LoadCombinationId_ModelId" ON "ResultSets" ("LoadCombinationId", "ModelId");

CREATE INDEX "IX_ResultSets_ModelId" ON "ResultSets" ("ModelId");

CREATE INDEX "IX_SectionProfileInfoBase_ModelId" ON "SectionProfileInfoBase" ("ModelId");

CREATE INDEX "IX_SectionProfileProposal_ModelId" ON "SectionProfileProposal" ("ModelId");

CREATE INDEX "IX_SectionProfileProposal_ModelProposalId_ModelId" ON "SectionProfileProposal" ("ModelProposalId", "ModelId");

CREATE INDEX "IX_SectionProfileProposalFromLibrary_ModelId" ON "SectionProfileProposalFromLibrary" ("ModelId");

CREATE INDEX "IX_SectionProfileProposalFromLibrary_ModelProposalId_ModelId" ON "SectionProfileProposalFromLibrary" ("ModelProposalId", "ModelId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250919211117_InitialCreateSqlite', '9.0.9');

COMMIT;

