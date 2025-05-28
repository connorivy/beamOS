using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.CodeGen.StructuralAnalysisApiClient;

public partial class InMemoryApiClient : IStructuralAnalysisApiClientV1
{
    private partial BeamOs.Common.Contracts.ModelResourceRequest<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.SectionProfileFromLibraryData> CreateCommand1(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.SectionProfileFromLibraryData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.Common.Contracts.ModelResourceRequest<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.SectionProfileFromLibraryData>
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.BatchPutSectionProfileFromLibraryCommand CreateCommand2(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.SectionProfileFromLibrary> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.BatchPutSectionProfileFromLibraryCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.CreateSectionProfileCommand CreateCommand3(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.CreateSectionProfileRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.CreateSectionProfileCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.BatchPutSectionProfileCommand CreateCommand4(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.PutSectionProfileRequest> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.BatchPutSectionProfileCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.Common.Contracts.IModelEntity CreateCommand5(
        System.Guid modelId,
        int id,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new ModelEntityResponse(id, modelId);
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.PutSectionProfileCommand CreateCommand6(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.SectionProfileData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.PutSectionProfileCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.PutSectionProfileFromLibraryCommand CreateCommand7(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles.SectionProfileFromLibraryData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles.PutSectionProfileFromLibraryCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads.CreatePointLoadCommand CreateCommand8(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads.CreatePointLoadRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads.CreatePointLoadCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads.BatchPutPointLoadCommand CreateCommand9(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads.PutPointLoadRequest> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads.BatchPutPointLoadCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads.PutPointLoadCommand CreateCommand10(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads.PointLoadData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads.PutPointLoadCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.CreateNodeCommand CreateCommand11(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes.CreateNodeRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.CreateNodeCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.PatchNodeCommand CreateCommand12(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes.UpdateNodeRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.PatchNodeCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.BatchPutNodeCommand CreateCommand13(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes.PutNodeRequest> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.BatchPutNodeCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.PutNodeCommand CreateCommand14(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes.NodeData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes.PutNodeCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads.CreateMomentLoadCommand CreateCommand15(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads.CreateMomentLoadRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads.CreateMomentLoadCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads.BatchPutMomentLoadCommand CreateCommand16(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads.PutMomentLoadRequest> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads.BatchPutMomentLoadCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads.PutMomentLoadCommand CreateCommand17(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads.MomentLoadData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads.PutMomentLoadCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.CreateModelRequest CreateCommand18(
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.CreateModelRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return body;
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.EmptyRequest CreateCommand19(
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.EmptyRequest { };
    }

    private partial BeamOs.Common.Contracts.ModelResourceRequest<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.ModelProposalData> CreateCommand20(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.ModelProposalData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.Common.Contracts.ModelResourceRequest<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.ModelProposalData>
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial System.Guid CreateCommand21(
        System.Guid modelId,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return modelId;
    }

    private partial BeamOs.Common.Contracts.ModelResourceRequest<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.ModelInfoData> CreateCommand22(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.ModelInfoData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.Common.Contracts.ModelResourceRequest<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models.ModelInfoData>
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.Common.Contracts.ModelResourceRequest<string> CreateCommand23(
        System.Guid modelId,
        string body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.Common.Contracts.ModelResourceRequest<string>
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials.CreateMaterialCommand CreateCommand24(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials.CreateMaterialRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials.CreateMaterialCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials.BatchPutMaterialCommand CreateCommand25(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials.PutMaterialRequest> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials.BatchPutMaterialCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials.PutMaterialCommand CreateCommand26(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials.MaterialRequestData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials.PutMaterialCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations.BatchPutLoadCombinationCommand CreateCommand27(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations.BatchPutLoadCombinationCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations.CreateLoadCombinationCommand CreateCommand28(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombinationData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations.CreateLoadCombinationCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations.PutLoadCombinationCommand CreateCommand29(
        System.Guid modelId,
        int id,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombinationData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations.PutLoadCombinationCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases.BatchPutLoadCaseCommand CreateCommand30(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCase> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases.BatchPutLoadCaseCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases.CreateLoadCaseCommand CreateCommand31(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCaseData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases.CreateLoadCaseCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases.PutLoadCaseCommand CreateCommand32(
        System.Guid modelId,
        int id,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCaseData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases.PutLoadCaseCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.CreateElement1dCommand CreateCommand33(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds.CreateElement1dRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.CreateElement1dCommand
        {
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.BatchPutElement1dCommand CreateCommand34(
        System.Guid modelId,
        System.Collections.Generic.IEnumerable<BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds.PutElement1dRequest> body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.BatchPutElement1dCommand
        {
            ModelId = modelId,
            Body = body.ToArray(),
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.PutElement1dCommand CreateCommand35(
        int id,
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds.Element1dData body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.PutElement1dCommand
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod.RunDsmCommand CreateCommand36(
        System.Guid modelId,
        BeamOs.StructuralAnalysis.Contracts.Common.RunDsmRequest body,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod.RunDsmCommand
        {
            ModelId = modelId,
            UnitsOverride = body.UnitsOverride,
            LoadCombinationIds = body.LoadCombinationIds,
        };
    }

    private partial BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate.ModelId CreateCommand37(
        System.Guid modelId,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return modelId;
    }

    private partial BeamOs.StructuralAnalysis.Application.AnalyticalResults.GetDiagramsCommand CreateCommand38(
        System.Guid modelId,
        int id,
        string unitsOverride,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.AnalyticalResults.GetDiagramsCommand
        {
            ModelId = modelId,
            Id = id,
            UnitsOverride = unitsOverride,
        };
    }

    private partial BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.GetAnalyticalResultResourceQuery CreateCommand39(
        System.Guid modelId,
        int resultSetId,
        int id,
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds.GetAnalyticalResultResourceQuery
        {
            ModelId = modelId,
            ResultSetId = resultSetId,
            Id = id,
        };
    }
}
