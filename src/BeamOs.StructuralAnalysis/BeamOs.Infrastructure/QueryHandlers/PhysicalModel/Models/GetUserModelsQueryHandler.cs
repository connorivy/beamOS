using System.Security.Claims;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal class GetUserModelsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<ClaimsPrincipal, List<ModelResponse>>
{
    public async Task<List<ModelResponse>?> ExecuteAsync(
        ClaimsPrincipal query,
        CancellationToken ct = default
    )
    {
        var models = dbContext.Models.AsAsyncEnumerable();

        List<ModelResponse> modelResponses = [];
        await foreach (var model in models)
        {
            var mapper = ModelToModelResponseMapper.Create(model.Settings.UnitSettings);
            modelResponses.Add(mapper.Map(model));
        }

        return modelResponses;
    }
}
