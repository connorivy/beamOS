using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Application.DesignCodes;

public sealed class OptimizeSectionsCommandHandler(
    IModelRepository modelRepository,
    IEnvelopeResultSetRepository resultSetRepository,
    IStructuralAnalysisUnitOfWork unitOfWork,
    ILogger<OptimizeSectionsCommandHandler> logger
) : ICommandHandler<Guid, string>
{
    public async Task<Result<string>> ExecuteAsync(Guid command, CancellationToken ct = default)
    {
        var model = await modelRepository.GetSingle(
            command,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds),
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.SectionProfile)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.Material)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.StartNode)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.EndNode)}",
            $"{nameof(Model.Nodes)}.{nameof(Node.PointLoads)}",
            $"{nameof(Model.Nodes)}.{nameof(Node.MomentLoads)}"
        );

        if (model is null)
        {
            return BeamOsError.NotFound(description: $"Could not find model with id {command}");
        }

        // var envelopeResultSet = await resultSetRepository.GetSingle(command);
        if (model.LoadCombinations is null || model.LoadCombinations.Count == 0)
        {
            return BeamOsError.NotFound(
                description: $"Model with id {command} has no load combinations"
            );
        }
        return string.Empty;
    }
}
