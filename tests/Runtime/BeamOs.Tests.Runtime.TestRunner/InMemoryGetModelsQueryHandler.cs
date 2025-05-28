using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.Tests.Runtime.TestRunner;

public class InMemoryGetModelsQueryHandler(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
) : IQueryHandler<EmptyRequest, List<ModelInfoResponse>>
{
    public Task<Result<List<ModelInfoResponse>>> ExecuteAsync(
        EmptyRequest query,
        CancellationToken ct = default
    )
    {
        var models = inMemoryModelRepositoryStorage.Models.Values;

        Result<List<ModelInfoResponse>> result = models
            .Select(m => new ModelInfoResponse(
                m.Id,
                m.Name,
                m.Description,
                m.Settings.ToContract(),
                m.LastModified,
                "Owner"
            ))
            .ToList();
        return Task.FromResult(result);
    }
}
