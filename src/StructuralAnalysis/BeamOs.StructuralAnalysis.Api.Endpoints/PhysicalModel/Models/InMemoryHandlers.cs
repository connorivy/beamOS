using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

public class InMemoryGetModelsQueryHandler(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
) : IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>>
{
    public Task<Result<ICollection<ModelInfoResponse>>> ExecuteAsync(
        EmptyRequest query,
        CancellationToken ct = default
    )
    {
        var models = inMemoryModelRepositoryStorage.Models.Values;

        Result<ICollection<ModelInfoResponse>> result = models
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

public sealed class InMemoryRestoreModeCommandHandler()
    : ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>
{
    public Task<Result<ModelResponse>> ExecuteAsync(
        ModelResourceRequest<DateTimeOffset> command,
        CancellationToken ct = default
    )
    {
        throw new NotImplementedException("InMemoryRestoreModeCommandHandler is not implemented.");
    }
}
