using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Application.AnalyticalResults.ModelResults;
using BeamOs.Application.AnalyticalResults.NodeResults;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.Application.DirectStiffnessMethod.Commands;

public class RunDirectStiffnessMethodCommandHandler(
    IModelRepository modelRepository,
    IModelResultRepository modelResultRepository,
    INodeResultRepository nodeResultRepository,
    IShearDiagramRepository shearDiagramRepository,
    IMomentDiagramRepository momentDiagramRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<RunDirectStiffnessMethodCommand, ModelResults>
{
    public async Task<ModelResults> ExecuteAsync(
        RunDirectStiffnessMethodCommand command,
        CancellationToken ct = default
    )
    {
        var model = await modelRepository.GetById(
            command.ModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds),
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.SectionProfile)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.Material)}",
            $"{nameof(Model.Nodes)}.{nameof(Node.PointLoads)}",
            $"{nameof(Model.Nodes)}.{nameof(Node.MomentLoads)}"
        );
        var dsmModel = new DsmAnalysisModel(model);

        ModelResults results = dsmModel.RunAnalysis();

        foreach (var nodeResult in results.NodeResults ?? Enumerable.Empty<NodeResult>())
        {
            nodeResultRepository.Add(nodeResult);
        }

        foreach (
            var shearForceDiagram in results.ShearForceDiagrams
                ?? Enumerable.Empty<ShearForceDiagram>()
        )
        {
            shearDiagramRepository.Add(shearForceDiagram);
        }

        foreach (var momentDiagram in results.MomentDiagrams ?? Enumerable.Empty<MomentDiagram>())
        {
            momentDiagramRepository.Add(momentDiagram);
        }

        modelResultRepository.Add(
            new ModelResult(
                model.Id,
                results.MaxShearValue,
                results.MinShearValue,
                results.MaxMomentValue,
                results.MinMomentValue
            )
        );

        await unitOfWork.SaveChangesAsync(ct);

        return results;
    }
}
