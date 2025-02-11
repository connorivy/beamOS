using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.Extensions.Logging;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;

public class RunDirectStiffnessMethodCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository,
    IStructuralAnalysisUnitOfWork unitOfWork,
    ILogger<RunDirectStiffnessMethodCommandHandler> logger
) : ICommandHandler<RunDsmCommand, AnalyticalResultsResponse>
{
    public async Task<Result<AnalyticalResultsResponse>> ExecuteAsync(
        RunDsmCommand command,
        CancellationToken ct = default
    )
    {
        UnitSettings? unitSettings = null;
        if (command.UnitsOverride is not null)
        {
            unitSettings = GetUnitSettings(command.UnitsOverride);
        }

        var model = await modelRepository.GetSingle(
            command.ModelId,
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

        var dsmModel = new DsmAnalysisModel(model, unitSettings);

        var analysisResults = dsmModel.RunAnalysis();

        resultSetRepository.Add(analysisResults.ResultSet);

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

        return analysisResults.OtherAnalyticalResults.Map();
    }

    private static UnitSettings GetUnitSettings(string unitsOverride) =>
        unitsOverride.ToLowerInvariant() switch
        {
            "k_in" or "k-in" => UnitSettings.K_IN,
            "k_ft" or "k-ft" => UnitSettings.K_FT,
            "kn_m" or "kn-m" => UnitSettings.kN_M,
            _
                => throw new ArgumentException(
                    $"Invalid units override, {unitsOverride}",
                    nameof(unitsOverride)
                )
        };
}

public readonly struct RunDsmCommand : IHasModelId
{
    public Guid ModelId { get; init; }

    public string? UnitsOverride { get; init; }
}

[Mapper]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class DiagramToResponseMapper
{
    public static partial AnalyticalResultsResponse Map(this OtherAnalyticalResults diagram);
}
