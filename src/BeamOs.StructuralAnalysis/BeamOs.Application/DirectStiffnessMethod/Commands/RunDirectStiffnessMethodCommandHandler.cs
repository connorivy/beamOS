using BeamOs.Application.AnalyticalModel.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Application.AnalyticalModel.ModelResults;
using BeamOs.Application.AnalyticalModel.NodeResults;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Models;
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
) : ICommandHandler<RunDirectStiffnessMethodCommand, bool>
{
    public async Task<bool> ExecuteAsync(
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

        Domain.AnalyticalModel.AnalyticalResultsAggregate.AnalyticalResults analyticalModelResults =
            dsmModel.RunAnalysis();

        modelResultRepository.Add(analyticalModelResults);

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
