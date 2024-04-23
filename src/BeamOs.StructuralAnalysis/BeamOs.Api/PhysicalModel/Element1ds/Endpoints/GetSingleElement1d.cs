using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Application.PhysicalModel.Models.Interfaces;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.PhysicalModel.Common.Extensions;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetSingleElement1d(
    IQueryHandler<GetResourceByIdWithPropertiesQuery, IElement1dData> getElement1dQueryHandler,
    IQueryHandler<GetResourceByIdWithPropertiesQuery, IModelData> getModelQueryHandler,
    IElement1dDataToResponseMapper element1dDataMapper
) : Endpoint<IdRequest, Element1dResponseHydrated?>
{
    public override void Configure()
    {
        this.Get("element1Ds/{id}");
        this.AllowAnonymous();
    }

    public override async Task<Element1dResponseHydrated?> ExecuteAsync(
        IdRequest req,
        CancellationToken ct
    )
    {
        GetResourceByIdWithPropertiesQuery query = new(Guid.Parse(req.Id));

        IElement1dData? data = await getElement1dQueryHandler.ExecuteAsync(query, ct);

        if (data is null)
        {
            return null;
        }

        var modelData = await getModelQueryHandler.ExecuteAsync(new(data.ModelId, []), ct);
        data.UseUnitSettings(modelData.Settings.UnitSettings);

        return element1dDataMapper.Map(data);
    }
}
