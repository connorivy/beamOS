using System.Reflection;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.OpenSees;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Application.OpenSees;

public sealed class RunOpenSeesCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository,
    IStructuralAnalysisUnitOfWork unitOfWork,
    ILogger<RunOpenSeesCommandHandler> logger
) : ICommandHandler<RunDsmCommand, AnalyticalResultsResponse>
{
    public async Task<Result<AnalyticalResultsResponse>> ExecuteAsync(
        RunDsmCommand command,
        CancellationToken ct = default
    )
    {
        var model = await modelRepository.GetSingle(
            command.ModelId,
            static queryable =>
                queryable
                    .Include(m => m.PointLoads)
                    .Include(m => m.MomentLoads)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.StartNode)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.EndNode)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.SectionProfile)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.Material),
            ct
        );

        if (model is null)
        {
            return BeamOsError.NotFound(
                description: $"Could not find model with id {command.ModelId}"
            );
        }

        UnitSettings unitSettings = model.Settings.UnitSettings;
        if (command.UnitsOverride is not null)
        {
            unitSettings = RunDirectStiffnessMethodCommandHandler.GetUnitSettings(
                command.UnitsOverride
            );
        }

        using OpenSeesAnalysisModel analysisMode = new(model, unitSettings, logger);
        var results = await analysisMode.RunAnalysis();

        resultSetRepository.Add(results.ResultSet);

        await unitOfWork.SaveChangesAsync(ct);

        return results.OtherAnalyticalResults.Map();
    }
}
