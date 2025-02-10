using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;

public class RunDirectStiffnessMethodCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository,
    IStructuralAnalysisUnitOfWork unitOfWork,
    ILogger<RunDirectStiffnessMethodCommandHandler> logger
) : ICommandHandler<ModelId, bool>
{
    public async Task<Result<bool>> ExecuteAsync(ModelId modelId, CancellationToken ct = default)
    {
        var model = await modelRepository.GetSingle(
            modelId,
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
        var dsmModel = new DsmAnalysisModel(model);

        var resultSet = dsmModel.RunAnalysis();

        resultSetRepository.Add(resultSet);

        //foreach (var nodeResult in results.NodeResults ?? Enumerable.Empty<NodeResult>())
        //{
        //    nodeResultRepository.Add(nodeResult);
        //}

        //foreach (
        //    var shearForceDiagram in results.ShearForceDiagrams
        //        ?? Enumerable.Empty<ShearForceDiagram>()
        //)
        //{
        //    shearDiagramRepository.Add(shearForceDiagram);
        //}

        //foreach (var momentDiagram in results.MomentDiagrams ?? Enumerable.Empty<MomentDiagram>())
        //{
        //    momentDiagramRepository.Add(momentDiagram);
        //}

        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}
