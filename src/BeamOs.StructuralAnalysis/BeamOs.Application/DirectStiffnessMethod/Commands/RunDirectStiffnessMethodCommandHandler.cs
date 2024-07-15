using BeamOs.Application.AnalyticalResults.NodeResults;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.Application.DirectStiffnessMethod.Commands;

public class RunDirectStiffnessMethodCommandHandler(
    IModelRepository modelRepository,
    INodeResultRepository nodeResultRepository,
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

        foreach (var nodeResult in results.NodeResults)
        {
            nodeResultRepository.Add(nodeResult);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return results;
    }
}
