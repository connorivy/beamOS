using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;

public class CreateAnalyticalModelCommandHandler(
    ICommandHandler<CreateAnalyticalNodeCommand, AnalyticalNode> createNodeHandler,
    ICommandHandler<CreateAnalyticalElement1dCommand, AnalyticalElement1D> createEl1dHandler)
    : ICommandHandler<CreateAnalyticalModelCommand, AnalyticalModel>
{
    public async Task<AnalyticalModel> ExecuteAsync(CreateAnalyticalModelCommand command, CancellationToken ct = default)
    {
        HashSet<AnalyticalNode> nodes = [];
        HashSet<AnalyticalElement1D> element1Ds = [];
        foreach (CreateAnalyticalNodeCommand nodeCommand in command.Nodes)
        {
            _ = nodes.Add(await createNodeHandler.ExecuteAsync(nodeCommand, ct));
        }

        foreach (CreateAnalyticalElement1dCommand el1dCommand in command.Element1Ds)
        {
            _ = element1Ds.Add(await createEl1dHandler.ExecuteAsync(el1dCommand, ct));
        }

        var model = AnalyticalModel.RunAnalysis(null, element1Ds, nodes);
        //var model = command.ToDomainObject();

        //await modelRepository.Add(model);

        //return model;
        return null;
    }
}
