using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal sealed class GetModelCommandHandler(IModelRepository modelRepository)
    : ICommandHandler<Guid, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        Guid command,
        CancellationToken ct = default
    )
    {
        var model = await modelRepository.GetSingle(
            command,
            ct,
            nameof(Model.Nodes),
            nameof(Model.InternalNodes),
            nameof(Model.PointLoads),
            nameof(Model.MomentLoads),
            nameof(Model.Element1ds),
            nameof(Model.SectionProfiles),
            nameof(Model.Materials),
            nameof(Model.ResultSets),
            nameof(Model.ResultSets) + "." + nameof(ResultSet.NodeResults),
            nameof(Model.LoadCases),
            nameof(Model.LoadCombinations)
        );
        if (model is null)
        {
            return BeamOsError.NotFound(description: $"Model with id {command} not found");
        }

        var mapper = ModelToResponseMapper.Create(model.Settings.UnitSettings);
        return mapper.Map(model);
    }
}
