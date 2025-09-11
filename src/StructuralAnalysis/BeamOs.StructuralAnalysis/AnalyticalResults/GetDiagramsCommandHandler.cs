using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults;

public sealed class GetDiagramsCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository
) : ICommandHandler<GetDiagramsCommand, AnalyticalResultsResponse>
{
    public async Task<Result<AnalyticalResultsResponse>> ExecuteAsync(
        GetDiagramsCommand command,
        CancellationToken ct = default
    )
    {
        ResultSetId resultSetId = new(command.Id);
        var elementResults = await resultSetRepository.GetSingle(
            command.ModelId,
            resultSetId,
            ct,
            $"{nameof(ResultSet.Element1dResults)}",
            $"{nameof(ResultSet.NodeResults)}"
        );

        if (elementResults is null)
        {
            return BeamOsError.NotFound(
                description: $"Could not find result set with id = {command.Id} for model {command.ModelId}"
            );
        }

        var model = await modelRepository.GetSingle(
            command.ModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds),
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.SectionProfile)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.Material)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.StartNode)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.EndNode)}"
        //$"{nameof(Model.Nodes)}.{nameof(Node.PointLoads)}",
        //$"{nameof(Model.Nodes)}.{nameof(Node.MomentLoads)}"
        );

        var dsmElement1ds =
            model.Settings.AnalysisSettings.Element1DAnalysisType == Element1dAnalysisType.Euler
                ? model
                    .Element1ds.Select(el => new DsmElement1d(
                        el,
                        el.SectionProfile.GetSectionProfile()
                    ))
                    .ToArray()
                : model
                    .Element1ds.Select(el => new TimoshenkoDsmElement1d(
                        el,
                        el.SectionProfile.GetSectionProfile()
                    ))
                    .ToArray();

        UnitSettings unitSettings;
        if (command.UnitsOverride is not null)
        {
            unitSettings = RunDirectStiffnessMethodCommandHandler.GetUnitSettings(
                command.UnitsOverride
            );
        }
        else
        {
            unitSettings = model.Settings.UnitSettings;
        }

        return elementResults.ComputeDiagramsFromExistingResults(dsmElement1ds, unitSettings).Map();
    }
}

public readonly struct GetDiagramsCommand : IHasModelId
{
    public Guid ModelId { get; init; }

    public int Id { get; init; }

    public string? UnitsOverride { get; init; }
}
